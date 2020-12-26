
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class BrutalBusinessPracticesCardController : MrNobleCardController
	{
		public BrutalBusinessPracticesCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => IsVillainTarget(c) && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);
			AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target.IsEnvironmentTarget, (DealDamageAction dd) => 3);
		}
	}
}
