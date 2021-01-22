using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class NeverToldYouALieCardController : ConmanUtilityCardController
    {

        public NeverToldYouALieCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //"Deal 2 targets 2 psychic damage. Those targets cannot deal damage until {Conman}'s next turn.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Psychic, 2, false, 2, addStatusEffect: (DealDamageAction dd) => ThoseTargetsCannotDealDamageUntilTheStartOfNextTurnResponse(dd), selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
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

        protected IEnumerator ThoseTargetsCannotDealDamageUntilTheStartOfNextTurnResponse(DealDamageAction dealDamage)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = dealDamage.Target;
            cannotDealDamageStatusEffect.UntilStartOfNextTurn(TurnTaker);
            cannotDealDamageStatusEffect.UntilCardLeavesPlay(dealDamage.Target);
            IEnumerator coroutine = AddStatusEffect(cannotDealDamageStatusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
            
        }

    }
}