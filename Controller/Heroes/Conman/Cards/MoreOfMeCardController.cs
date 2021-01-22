using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class MoreOfMeCardController : ConmanUtilityCardController
    {

        public MoreOfMeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public override void AddTriggers()
        {
            //"If there is a Changeling card in play, {Conman} may use an extra power on his turn.",
            AddAdditionalPhaseActionTrigger((TurnTaker tt) => ShouldIncreasePhaseActionCount(tt) & IsChangelingCardInPlay(), Phase.UsePower, 1);
            //"Whenever a Changeling card would be destroyed, you may destroy this card instead."
            AddTrigger((DestroyCardAction dca) => dca.CardToDestroy != null && IsChangeling(dca.CardToDestroy.Card) && GameController.IsCardVisibleToCardSource(dca.CardToDestroy.Card, GetCardSource()), DestroyThisCardInsteadResponse, new TriggerType[]
                {
                    TriggerType.CancelAction,
                    TriggerType.DestroySelf
                }, TriggerTiming.Before);
        }

        private IEnumerator DestroyThisCardInsteadResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine = GameController.DestroyCard(HeroTurnTakerController, Card, optional: true, actionSource: dca, postDestroyAction: () => CancelAction(dca), associatedCards: dca.CardToDestroy.Card.ToEnumerable().ToList(), cardSource: GetCardSource());
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

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == base.TurnTaker;
        }

        public override IEnumerator Play()
        {
            if(IsChangelingCardInPlay())
            {
                IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1);
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

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            if (base.GameController.ActiveTurnPhase.IsUsePower && IsChangelingCardInPlay())
            {
                return ShouldIncreasePhaseActionCount(base.GameController.ActiveTurnTaker);
            }
            return false;
        }

    }
}