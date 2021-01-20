using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TheDragonRune.Conman
{
	public class ConmanCharacterCardController : HeroCharacterCardController
	{
		public ConmanCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		private ITrigger _reduceTrigger;
		public override IEnumerator UsePower(int index = 0)
		{
			//The next time {Conman} would take Damage, redirect it to another  hero target and reduce the damage by 2.
			int reducePowerNumeral = GetPowerNumeral(0, 2);

			int[] powerNumerals = new int[]
			{
				reducePowerNumeral
			};
			OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, "RedirectAndLowerResponse", $"The next time {CharacterCard.Title} would take Damage, redirect it to another  hero target and reduce the damage by 2.", new TriggerType[]
			{
			TriggerType.RedirectDamage,
			TriggerType.ReduceDamage
			}, base.TurnTaker, base.Card, powerNumerals);
			onDealDamageStatusEffect.TargetCriteria.IsSpecificCard = CharacterCard;
			onDealDamageStatusEffect.DamageAmountCriteria.GreaterThan = 0;
			onDealDamageStatusEffect.NumberOfUses = 1;
			onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.Before;
			IEnumerator coroutine = AddStatusEffect(onDealDamageStatusEffect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}


		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						
						break;
					}
				case 1:
					{
						break;
					}
				case 2:
					{
						break;
					}
			}
			yield break;
		}

		public IEnumerator RedirectAndLowerResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
		{
			int? num = null;
			if (powerNumerals != null)
			{
				num = powerNumerals.ElementAtOrDefault(0);
			}
			if (!num.HasValue)
			{
				num = 2;
			}

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.RedirectDamage, new LinqCardCriteria(c => c.IsHero && c.IsTarget && c != Card && c.IsInPlayAndHasGameText, "hero target"), storedResults, false, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if(DidSelectCard(storedResults))
            {
				Card target = GetSelectedCard(storedResults);
				IEnumerator redirect = base.GameController.RedirectDamage(dd, target, cardSource: GetCardSource());
				IEnumerator reduce = base.GameController.ReduceDamage(dd, num.Value, _reduceTrigger, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(redirect);
					yield return base.GameController.StartCoroutine(reduce);
				}
				else
				{
					base.GameController.ExhaustCoroutine(redirect);
					base.GameController.ExhaustCoroutine(reduce);
				}
			}
			
			yield break;
		}
	}
}
