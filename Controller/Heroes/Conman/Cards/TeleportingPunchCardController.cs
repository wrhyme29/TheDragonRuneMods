using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class TeleportingPunchCardController : ConmanUtilityCardController
    {

        public TeleportingPunchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal 2 Melee Damage to 2 different Targets.
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 2, false, 2, cardSource: GetCardSource());
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