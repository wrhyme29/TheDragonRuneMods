
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class AndvariSoldiersCardController : MrNobleCardController
	{
		public AndvariSoldiersCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
		}

		public override void AddTriggers()
		{
			AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.LowestHP, 1, DamageType.Melee);
			AddRedirectDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard || IsHighTierEnforcer(dd.Target), () => base.Card);
		}
	}
}
