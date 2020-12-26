using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class MrNobleTurnTakerController : TurnTakerController
	{
		public MrNobleTurnTakerController(TurnTaker turnTaker, GameController gameController)
			: base(turnTaker, gameController)
		{
		}

		public override IEnumerator StartGame()
		{
			if (base.CharacterCardController is MrNobleCharacterCardController)
			{
				IEnumerator high = PutCardsIntoPlay(new LinqCardCriteria((Card c) => IsHighTierEnforcer(c), "high tier enforcer"), 1, shuffleDeckAfter: false);
				IEnumerator low = PutCardsIntoPlay(new LinqCardCriteria((Card c) => IsLowerTierEnforcer(c), "lower tier enforcer"), base.H - 3);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(high);
					yield return base.GameController.StartCoroutine(low);
				}
				else
				{
					base.GameController.ExhaustCoroutine(high);
					base.GameController.ExhaustCoroutine(low);
				}
			}
		}

		private bool IsHighTierEnforcer(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "high tier enforcer");
		}

		private bool IsLowerTierEnforcer(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "lower tier enforcer");
		}
	}
}