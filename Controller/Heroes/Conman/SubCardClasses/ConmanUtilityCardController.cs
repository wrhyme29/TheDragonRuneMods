using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TheDragonRune.Conman
{
    public class ConmanUtilityCardController : CardController
    {

        public ConmanUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public static readonly string ChangelingKeyword = "changeling";

		protected bool IsChangeling(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, ChangelingKeyword);
		}


		protected IEnumerable<Card> GetChangelingCardsInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsChangeling(c));
		}

		protected bool IsChangelingCardInPlay()
		{
			return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsChangeling(c)).Any();
		}

	}
}