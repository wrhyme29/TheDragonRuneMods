
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class RecruitmentDriveCardController : MrNobleCardController
	{
		public RecruitmentDriveCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine2 = RevealCards_PutSomeIntoPlay_DiscardRemaining(base.TurnTakerController, base.TurnTaker.Deck, base.H, new LinqCardCriteria((Card c) => IsEnforcer(c), "enforcer"));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			coroutine2 = ShuffleDeck(DecisionMaker, base.TurnTaker.Deck);
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
