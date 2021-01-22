using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class TooCloseToHomeCardController : ConmanUtilityCardController
    {

        public TooCloseToHomeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn(FirstTimeDamageDealt), () => "A villain target has dealt damage this turn.", () => "A villain target has not dealt damage this turn.").Condition = () => Card.IsInPlayAndHasGameText;
        }

		private ITrigger _reduceTrigger;

		public static readonly string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //Reduce the first damage dealt each turn by a Villain target by 1.
            AddTrigger((DealDamageAction dd) => !HasBeenSetToTrueThisTurn(FirstTimeDamageDealt) && dd.DamageSource != null && IsVillainTarget(dd.DamageSource.Card), ReduceResponse, TriggerType.ReduceDamage, TriggerTiming.Before);

        }

		public IEnumerator ReduceResponse(DealDamageAction dd)
		{
			SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealt);
			IEnumerator reduce = base.GameController.ReduceDamage(dd, 1, _reduceTrigger, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(reduce);
			}
			else
			{
				base.GameController.ExhaustCoroutine(reduce);
			}
			
			yield break;
		}
	}
}