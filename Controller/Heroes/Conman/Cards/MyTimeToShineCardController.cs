using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class MyTimeToShineCardController : ConmanUtilityCardController
    {

        public MyTimeToShineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override bool DoNotMoveOneShotToTrash
        {
            get
            {
                if (Card.IsInHand)
                {
                    return true;
                }
                return false;
            }
        }

        public override IEnumerator Play()
        {
            //Shuffle all cards in each trash into their respective decks.
            int numCardsInTrashes = 0;
            IEnumerator coroutine;
            foreach (TurnTaker tt in GameController.AllTurnTakers.Where(tt => !tt.IsIncapacitatedOrOutOfGame))
            {
                numCardsInTrashes += tt.Trash.Cards.Count();
                coroutine = base.GameController.ShuffleTrashIntoDeck(FindTurnTakerController(tt), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            string message;
            if(numCardsInTrashes > 1)
            {
                message = $"{numCardsInTrashes} cards were shuffled into their decks!";
            } else if(numCardsInTrashes == 1)
            {
                message = $"{numCardsInTrashes} card was shuffled into its deck!";

            } else
            {
                message = $"No cards were shuffled in their decks!";
            }
            coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //For every 3 cards shuffled from the trash this way, {Conman} may deal up to that many targets 2 melee damage, and 1 psychic damage.
            if (numCardsInTrashes >= 3)
            {
                int every3 = numCardsInTrashes / 3;
                List<DealDamageAction> list = new List<DealDamageAction>();
                list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 2, DamageType.Melee));
                list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Psychic));
                coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, minNumberOfTargets: 0, maxNumberOfTargets: every3);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Gain X hp, where X is the number of cards shuffled this way / 5
            if(numCardsInTrashes >= 5)
            {
                int every5 = numCardsInTrashes / 5;
                coroutine = GameController.GainHP(CharacterCard, every5, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //If at least 40 cards are shuffled this way, return this card to your hand instead of it going into the trash
            if(numCardsInTrashes >= 40)
            {
                IEnumerator messageE = GameController.SendMessageAction(Card.Title + " returns itself to " + TurnTaker.Name + "'s hand.", Priority.Low, GetCardSource(), showCardSource: true);
                coroutine = GameController.MoveCard(TurnTakerController, Card, HeroTurnTaker.Hand, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageE);
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageE);
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }


    }
}