
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class FrostFlameBrothersCardController : MrNobleCardController
	{
		private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

		public FrostFlameBrothersCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn("FirstTimeDamageDealt"), () => "An enforcer has been dealt damage this turn.", () => "An enforcer has not been dealt damage this turn.").Condition = () => base.Game.HasGameStarted;
		}

		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => IsEnforcer(dd.Target) && dd.DidDealDamage && !HasBeenSetToTrueThisTurn("FirstTimeDamageDealt"), DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
			AddReduceDamageTrigger((Card c) => IsEnforcer(c) && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);
		}

		private IEnumerator DealDamageResponse(DealDamageAction arg)
		{
			SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt");
			IEnumerator coroutine = DealMultipleInstancesOfDamage(new List<DealDamageAction>
		{
			new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), null, 1, DamageType.Fire),
			new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), null, 1, DamageType.Cold)
		}, (Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()));
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