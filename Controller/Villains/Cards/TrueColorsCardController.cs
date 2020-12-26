
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace TheDragonRune.MrNoble
{
	public class TrueColorsCardController : MrNobleCardController
	{
		public TrueColorsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
		}

		public override IEnumerator Play()
		{
			List<Card> usedSources = new List<Card>();
			IEnumerable<Card> enforcersInPlay = FindEnforcersInPlay();
			while (enforcersInPlay.Count() > 0)
			{
				IEnumerable<Card> source = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && enforcersInPlay.Contains(c) && !usedSources.Contains(c));
				if (source.Count() == 0)
				{
					break;
				}
				Card enforcerSource = enforcersInPlay.First();
				IEnumerator coroutine2;
				if (enforcersInPlay.Count() > 1)
				{
					List<SelectCardDecision> storedTargetResults = new List<SelectCardDecision>();
					coroutine2 = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, source, storedTargetResults, optional: false, allowAutoDecide: true);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine2);
					}
					SelectCardDecision selectTargetDecision = storedTargetResults.FirstOrDefault();
					if (selectTargetDecision != null)
					{
						enforcerSource = GetSelectedCard(storedTargetResults);
					}
				}
				usedSources.Add(enforcerSource);
				coroutine2 = DealDamageToHighestHP(enforcerSource, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 1, DamageType.Melee);
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
}
