using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq ;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class SwitcherooStrikesCardController : ConmanUtilityCardController
    {

        public SwitcherooStrikesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Deal X targets 1 melee and 3 psychic damage, where X is the number of teleports in play and in { Conman}’s Trash."
            int X = FindCardsWhere(c => IsTeleport(c) && (c.Location.IsPlayArea || TurnTaker.Trash.HasCard(c))).Count();
            Func<int> dynX = () => FindCardsWhere(c => IsTeleport(c) && (c.Location.IsPlayArea || TurnTaker.Trash.HasCard(c))).Count();
            List<DealDamageAction> list = new List<DealDamageAction>();
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Melee));
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 3, DamageType.Psychic));
            IEnumerator coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, minNumberOfTargets: X, maxNumberOfTargets: X, dynamicNumberOfTargets: dynX);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

        }
    }
}