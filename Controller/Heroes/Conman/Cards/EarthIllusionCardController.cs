using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class EarthIllusionCardController : ChangelingCardController
    {

        public EarthIllusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //{Conman} may not Play card or use powers.
            CannotPlayCards((TurnTakerController ttc) => ttc == TurnTakerController);
            CannotUsePowers((TurnTakerController ttc) => ttc == TurnTakerController);

            //At the end of {Conman}’s turn, one other player may play a card or use a power.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.PlayCard,
                TriggerType.UsePower
            });
            //At the start of {Conman}’s turn, you may shuffle this card into the deck.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, StartOfTurnResponse, TriggerType.MoveCard);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.MakeDecision, false, false, storedResults, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt != TurnTaker, "active other heroes"), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            TurnTaker hero = GetSelectedTurnTaker(storedResults);
            if (hero == null || !hero.IsHero)
            {
                yield break;
            }
            HeroTurnTakerController httc = FindHeroTurnTakerController(hero.ToHero());
            List<Function> list = new List<Function>();
            Function item = new Function(DecisionMaker, "Play a card.", SelectionType.PlayCard, () => base.GameController.SelectAndPlayCardFromHand(httc, false, cardSource: GetCardSource()));
            list.Add(item);
            Function item2 = new Function(DecisionMaker, "Use a power", SelectionType.UsePower, () => base.GameController.SelectAndUsePower(httc, optional: false, cardSource: GetCardSource()));
            list.Add(item2);

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, httc, list, optional: true, cardSource: GetCardSource());
            coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //you may shuffle this card into the deck.

            IEnumerator coroutine = GameController.ShuffleCardIntoLocation(HeroTurnTakerController, Card, TurnTaker.Deck, true, cardSource: GetCardSource());
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
    }
}