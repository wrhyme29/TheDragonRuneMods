using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class HighTierEnforcerCardController : MrNobleCardController
	{
		protected DamageType DamageType1;

		protected DamageType DamageType2;

		public HighTierEnforcerCardController(Card card, TurnTakerController turnTakerController, DamageType damageType1, DamageType damageType2)
			: base(card, turnTakerController)
		{
			DamageType1 = damageType1;
			DamageType2 = damageType2;
		}

		public override void AddTriggers()
		{
			AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && (dd.DamageType == DamageType1 || dd.DamageType == DamageType2));
		}
	}
}