
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class TheGunnCoupleCardController : MrNobleCardController
	{
		public TheGunnCoupleCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
		}

		public override void AddTriggers()
		{
			AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHeroCharacterCard, TargetType.HighestHP, 2, DamageType.Projectile);
			AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHeroCharacterCard, TargetType.HighestHP, 2, DamageType.Projectile);
			AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != base.Card && IsEnforcer(dd.DamageSource.Card), 1);
		}
	}
}
