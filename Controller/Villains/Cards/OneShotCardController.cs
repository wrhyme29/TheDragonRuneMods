
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class OneShotCardController : HighTierEnforcerCardController
	{
		public OneShotCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController, DamageType.Melee, DamageType.Projectile)
		{
			base.SpecialStringMaker.ShowHeroCharacterCardWithHighestHP(2);
		}

		public override void AddTriggers()
		{
			AddTrigger((PlayCardAction action) => action.WasCardPlayed && IsVillain(action.CardToPlay) && action.Origin == base.TurnTaker.Deck, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
			base.AddTriggers();
		}

		private IEnumerator DealDamageResponse(PlayCardAction arg)
		{
			IEnumerator coroutine = DealDamageToHighestHP(base.Card, 2, (Card c) => c.IsHeroCharacterCard, (Card c) => 4, DamageType.Projectile, isIrreducible: true);
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
