using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TheDragonRune.Conman
{
    public class ImOutOfHereCardController : ConmanUtilityCardController
    {

        public ImOutOfHereCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildInitialDamageString()).Condition = () => Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDamageDealt).Condition = () => Card.IsInPlayAndHasGameText;
        }

        public static readonly string WasDealtDamageByCard = "WasDealtDamageByCard";
        public static readonly string EnterPlayRoundIndex = "EnterPlayRound";
        public static readonly string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override IEnumerator Play()
        {
            //"When this card enters play, {Conman} deals himself 5 psychic damage.
            List<DealDamageAction> storedResults = new List<DealDamageAction>() ;
            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 5, DamageType.Psychic, storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SetCardProperty(EnterPlayRoundIndex, Game.Round);
            if (DidDealDamage(storedResults) && storedResults.First().Target == CharacterCard)
            {
                SetCardPropertyToTrueIfRealAction(WasDealtDamageByCard);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"At the end of {Conman}’s next turn, destroy this card and shuffle it and {Conman}'s trash into {Conman}’s deck."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfNextTurnResponse, new TriggerType[]
                {
                    TriggerType.DestroyCard,
                    TriggerType.MoveCard
                }, additionalCriteria: (PhaseChangeAction pca) => IsNextTurn);
            //If {Conman} takes damage that way, the first time a turn {Conman} would take damage redirect it to the Villain Target with the highest hp and increase the damage by 1.",
            AddTrigger((DealDamageAction dd) => WasDamagedDealtToConman && !HasBeenSetToTrueThisTurn(FirstTimeDamageDealt), RedirectAndIncreaseResponse, new TriggerType[]
                {
                    TriggerType.RedirectDamage,
                    TriggerType.IncreaseDamage
                }, TriggerTiming.Before);

            ResetFlagAfterLeavesPlay(WasDealtDamageByCard);
        }

        private IEnumerator RedirectAndIncreaseResponse(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealt);
            //redirect it to the Villain Target with the highest hp and increase the damage by 1.
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) => IsVillainTarget(c), storeHighest: storedResults, gameAction: dd, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storedResults != null && storedResults.Any())
            {
                Card target = storedResults.FirstOrDefault();
                IEnumerator redirect = base.GameController.RedirectDamage(dd, target, cardSource: GetCardSource());
                IEnumerator increase = base.GameController.IncreaseDamage(dd, 1,cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(redirect);
                    yield return base.GameController.StartCoroutine(increase);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(redirect);
                    base.GameController.ExhaustCoroutine(increase);
                }
            }

            yield break;
        }
    

        private IEnumerator EndOfNextTurnResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = GameController.DestroyCard(HeroTurnTakerController, Card, postDestroyAction: () => GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource()), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        private bool IsNextTurn
        {
            get
            {
                return GetCardPropertyJournalEntryInteger(EnterPlayRoundIndex) != null && GetCardPropertyJournalEntryInteger(EnterPlayRoundIndex).Value == Game.Round - 1;
            }
        }

        private bool WasDamagedDealtToConman
        {
            get 
            {
                return GetCardPropertyJournalEntryBoolean(WasDealtDamageByCard) != null && GetCardPropertyJournalEntryBoolean(WasDealtDamageByCard).Value;
            }
        }

        private string BuildInitialDamageString()
        {
            string initialSpecial = $"{CharacterCard.Title} was ";
            if(!WasDamagedDealtToConman)
            {
                initialSpecial += "not ";
            }
            initialSpecial += "dealt damage when this card entered play.";

            return initialSpecial;
        }



    }
}