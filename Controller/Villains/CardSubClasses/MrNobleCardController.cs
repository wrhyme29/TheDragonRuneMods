using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class MrNobleCardController : CardController
	{
		public MrNobleCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		protected bool IsEnforcer(Card card)
		{
			return card != null && (base.GameController.DoesCardContainKeyword(card, "high tier enforcer") || base.GameController.DoesCardContainKeyword(card, "mid tier enforcer") || base.GameController.DoesCardContainKeyword(card, "lower tier enforcer"));
		}

		protected bool IsLowerTierEnforcer(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "lower tier enforcer");
		}

		protected bool IsHighTierEnforcer(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "high tier enforcer");
		}

		protected int GetNumberOfEnforcersInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsEnforcer(c)).Count();
		}

		protected int GetNumberOfLowerTierEnforcersInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsLowerTierEnforcer(c)).Count();
		}

		protected IEnumerable<Card> FindEnforcersInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsEnforcer(c));
		}

		protected int GetNumberOfHighTierEnforcersInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsHighTierEnforcer(c)).Count();
		}
	}
}