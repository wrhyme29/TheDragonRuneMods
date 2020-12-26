
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class TheLivingShadowCardController : HighTierEnforcerCardController
	{
		public TheLivingShadowCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController, DamageType.Psychic, DamageType.Energy)
		{
			base.SpecialStringMaker.ShowHeroWithMostCards(inHand: false);
		}

		public override void AddTriggers()
		{
			AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyCardsResponse, TriggerType.DestroyCard);
			base.AddTriggers();
		}

		private IEnumerator DestroyCardsResponse(PhaseChangeAction pca)
		{
			if (FindCardsWhere((Card c) => c.IsInPlay && c.IsHero && (IsEquipment(c) || c.IsOngoing)).Count() > 0)
			{
				List<TurnTaker> storedResults = new List<TurnTaker>();
				IEnumerator coroutine = FindHeroWithMostCardsInPlay(storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				TurnTaker most = storedResults.FirstOrDefault();
				if (most != null && most.IsHero)
				{
					coroutine = base.GameController.SelectAndDestroyCard(FindHeroTurnTakerController(most.ToHero()), new LinqCardCriteria((Card c) => c.Owner == most && (IsEquipment(c) || c.IsOngoing), "equipment or ongoing cards owned by " + most.Name, useCardsSuffix: false), optional: false, null, null, GetCardSource());
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
			else
			{
				IEnumerator coroutine3 = base.GameController.SendMessageAction("There are hero ongoing or equipment cards in play for " + base.Card.Title + " to destroy.", Priority.Low, GetCardSource(), null, showCardSource: true);
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
}
