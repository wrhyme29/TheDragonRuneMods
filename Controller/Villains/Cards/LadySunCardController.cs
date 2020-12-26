
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class LadySunCardController : HighTierEnforcerCardController
	{
		public LadySunCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController, DamageType.Infernal, DamageType.Radiant)
		{
		}

		public override void AddTriggers()
		{
			AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DiscardOrDealDamageResponse, new TriggerType[2]
			{
			TriggerType.DiscardCard,
			TriggerType.DealDamage
			});
			base.AddTriggers();
		}

		private IEnumerator DiscardOrDealDamageResponse(PhaseChangeAction arg)
		{
			List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
			IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(0, 1, storedResults, allowAutoDecideHeroes: true, null, showCounter: false, null, ignoreBattleZone: false, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			IEnumerable<HeroTurnTakerController> enumerable = FindActiveHeroTurnTakerControllers();
			if (storedResults.Count > 0)
			{
				enumerable = enumerable.Except(from d in storedResults
											   where d.WasCardDiscarded
											   select d.HeroTurnTakerController);
			}
			List<Card> targets = new List<Card>();
			foreach (HeroTurnTakerController item in enumerable)
			{
				if (item.HasMultipleCharacterCards)
				{
					List<Card> storedCharacter = new List<Card>();
					IEnumerator coroutine2 = FindCharacterCardToTakeDamage(item.TurnTaker, storedCharacter, base.Card, base.H - 1, DamageType.Infernal);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine2);
					}
					targets.AddRange(storedCharacter);
				}
				else
				{
					targets.Add(item.CharacterCard);
				}
			}
			IEnumerator coroutine3 = DealDamage(base.Card, (Card c) => targets.Contains(c), 3, DamageType.Fire);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine3);
			}
		}
	}
}