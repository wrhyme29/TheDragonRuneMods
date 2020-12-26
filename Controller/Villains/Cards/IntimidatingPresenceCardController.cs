
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class IntimidatingPresenceCardController : MrNobleCardController
	{
		public IntimidatingPresenceCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine2 = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing, "ongoing"), autoDecide: false, null, null, null, SelectionType.DestroyCard, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
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