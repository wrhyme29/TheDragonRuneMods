using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class InspiringAttackCardController : ConmanUtilityCardController
    {

        public InspiringAttackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal 1 psychic damage to a villain target. Each other hero targets gains 2 hp.
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 1, DamageType.Psychic, 1, false, 1, additionalCriteria: (Card c) => IsVillainTarget(c), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.GainHP(HeroTurnTakerController, (Card c) => c.IsHero && c.IsTarget && c != CharacterCard, 2, cardSource: GetCardSource());
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