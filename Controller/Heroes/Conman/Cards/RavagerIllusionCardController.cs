using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class RavagerIllusionCardController : ChangelingCardController
    {

        public RavagerIllusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by Hero targets by 1, and reduce damage dealt to {Conman} by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsHero && dd.DamageSource.IsTarget, 1);
            AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);
        }


    }
}