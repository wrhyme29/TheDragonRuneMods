
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class LandTakeoverCardController : MrNobleCardController
	{
		public LandTakeoverCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine2 = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsEnvironmentTarget, storedResults, null, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			Card card = storedResults.FirstOrDefault();
			List<DestroyCardAction> storedDestroy = new List<DestroyCardAction>();
			if (card != null)
			{
				coroutine2 = base.GameController.DestroyCard(DecisionMaker, card, optional: false, storedDestroy, null, null, null, null, null, null, null, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			if (DidDestroyCard(storedDestroy))
			{
				coroutine2 = base.GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => c.IsHero && c.IsTarget, 2, DamageType.Fire, isIrreducible: false, optional: false, null, null, null, selectTargetsEvenIfCannotPerformAction: false, null, null, evenIfCannotDealDamage: false, ignoreBattleZone: false, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			else
			{
				coroutine2 = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), autoDecide: false, null, null, null, SelectionType.DestroyCard, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			coroutine2 = PlayTheTopCardOfTheVillainDeckWithMessageResponse(null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}
	}
}
