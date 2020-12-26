
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class LeadershipOfGreedCardController : MrNobleCardController
	{
		public LeadershipOfGreedCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && IsVillainTarget(dd.DamageSource.Card), (DealDamageAction dd) => 2);
		}
	}
}
