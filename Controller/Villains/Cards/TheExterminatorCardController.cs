
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class TheExterminatorCardController : MrNobleCardController
	{
		public TheExterminatorCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
		}

		public override void AddTriggers()
		{
			AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHeroCharacterCard, TargetType.HighestHP, 3, DamageType.Toxic);
			AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card.IsEnvironmentTarget && dd.Target == base.CharacterCard);
		}
	}
}
