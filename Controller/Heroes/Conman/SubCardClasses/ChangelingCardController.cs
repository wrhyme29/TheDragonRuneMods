using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class ChangelingCardController : ConmanUtilityCardController
    {

        public ChangelingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} shuffles all Changeling cards besides itself back into the deck!", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            IEnumerable<Card> changelings = GetChangelingCardsInPlay().Where(c => c != Card);
            coroutine = ShuffleCardsIntoLocationWithCondition(HeroTurnTakerController, changelings, TurnTaker.Deck);
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

		public IEnumerator ShuffleCardsIntoLocationWithCondition(HeroTurnTakerController decisionMaker, IEnumerable<Card> cards, Location location)
		{
			List<MoveCardAction> storedMove = new List<MoveCardAction>();
            IEnumerator coroutine = GameController.MoveCards(decisionMaker, cards, location, showIndividualMessages: true, storedResultsAction: storedMove, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

            if (DidMoveCard(storedMove))
            {
                coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
		}


	}
}