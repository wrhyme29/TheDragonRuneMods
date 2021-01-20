using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TheDragonRune.Conman
{
    public class LastingIllusionCardController : ChangelingCardController
    {

        public LastingIllusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //If this card would be shuffled into the deck from play, you may discard a card instead.
            AddTrigger((MoveCardAction mca) => mca.CardToMove != null && mca.CardToMove == Card && mca.Destination != null && mca.Destination == TurnTaker.Deck && mca.Origin != null && mca.Origin.IsPlayArea, DiscardToCancelMove, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.CancelAction
            }, TriggerTiming.Before);
            AddTrigger((BulkMoveCardsAction bmca) => bmca.CardsToMove != null && bmca.CardsToMove.Contains(Card) && bmca.Destination != null && bmca.Destination == TurnTaker.Deck && bmca.FindOriginForCard(Card).IsPlayArea, DiscardToRemoveFromMove, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.CancelAction
            }, TriggerTiming.Before);
            //During {Conman}’s draw phase, one other hero may also draw a card.
            AddPhaseChangeTrigger((TurnTaker tt) => tt == TurnTaker, (Phase p) => p == Phase.DrawCard, (PhaseChangeAction pca) => true, DrawPhaseResponse, new TriggerType[]
            {
                TriggerType.DrawCard
            }, TriggerTiming.After);
        }

        private IEnumerator DrawPhaseResponse(PhaseChangeAction pca)
        {
            //one other hero may also draw a card.
            IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, additionalCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker), cardSource: GetCardSource());
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

        private IEnumerator DiscardToRemoveFromMove(BulkMoveCardsAction bmca)
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCards(HeroTurnTakerController, 1, false, 0, storedResults, gameAction: bmca, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidDiscardCards(storedResults))
            {
                bmca.RemoveCardsFromMove(Card.ToEnumerable());
                yield return null;
            }

            yield break;
        }

        private IEnumerator DiscardToCancelMove(MoveCardAction mca)
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCards(HeroTurnTakerController, 1, false, 0, storedResults, gameAction: mca, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDiscardCards(storedResults))
            {
                coroutine = CancelAction(mca);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }


    }
}