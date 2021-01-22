using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class ShadowedIllusionCardController : ChangelingCardController
    {

        public ShadowedIllusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to hero targets by 2 and increase damage dealt by {Conman} by 1.
            AddReduceDamageTrigger((Card c) => c.IsHero && c.IsTarget, 2);
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard), 1);
        }


    }
}