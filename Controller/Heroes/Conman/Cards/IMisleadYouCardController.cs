using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class IMisleadYouCardController : ConmanUtilityCardController
    {

        public IMisleadYouCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal 2 psychic damage to a target. Reduce the next damage they would deal by 1.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Psychic, 1, optional: false, 1, addStatusEffect: (DealDamageAction dd) => ReduceNextDamageDealtByThatTargetResponse(dd, 1), selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
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

        private IEnumerator ReduceNextDamageDealtByThatTargetResponse(DealDamageAction action, int amount)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
            reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = action.Target;
            reduceDamageStatusEffect.UntilTargetLeavesPlay(action.Target);
            reduceDamageStatusEffect.NumberOfUses = 1;
            IEnumerator coroutine = AddStatusEffect(reduceDamageStatusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

    }
}