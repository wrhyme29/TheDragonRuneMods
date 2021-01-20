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
            coroutine = GameController.ShuffleCardsIntoLocation(HeroTurnTakerController, changelings, TurnTaker.Deck, individualMoves: true, cardSource: GetCardSource());
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