// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// TheDragonRuneTests.MrNobleTests
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using TheDragonRune.MrNoble;

namespace TheDragonRuneTest
{

	[TestFixture]
	public class MrNobleTests : BaseTest
	{
		protected TurnTakerController MrNoble => FindVillain("MrNoble");

		private bool IsHighTierEnforcer(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "high tier enforcer");
		}

		private bool IsLowerTierEnforcer(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "lower tier enforcer");
		}

		private bool IsEnforcer(Card card)
		{
			return card != null && (base.GameController.DoesCardContainKeyword(card, "high tier enforcer") || base.GameController.DoesCardContainKeyword(card, "mid tier enforcer") || base.GameController.DoesCardContainKeyword(card, "lower tier enforcer"));
		}

		private void PreventEndOfTurnEffects(TurnTakerController ttc, Card cardToPrevent)
		{
			PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect();
			preventPhaseEffectStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
			preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = cardToPrevent;
			RunCoroutine(base.GameController.AddStatusEffect(preventPhaseEffectStatusEffect, showMessage: true, ttc.CharacterCardController.GetCardSource()));
		}

		private void PreventStartOfTurnEffects(TurnTakerController ttc, Card cardToPrevent)
		{
			PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect(Phase.Start);
			preventPhaseEffectStatusEffect.UntilEndOfNextTurn(ttc.TurnTaker);
			preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = cardToPrevent;
			RunCoroutine(base.GameController.AddStatusEffect(preventPhaseEffectStatusEffect, showMessage: true, ttc.CharacterCardController.GetCardSource()));
		}

		[Test]
		public void TestMrNobleLoadedProperly()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Megalopolis");
			StartGame();
			Assert.AreEqual((object)3, (object)base.GameController.TurnTakerControllers.Count());
			Assert.IsNotNull((object)MrNoble);
			Assert.IsInstanceOf(typeof(MrNobleCharacterCardController), (object)MrNoble.CharacterCardController);
			Assert.AreEqual((object)67, (object)MrNoble.CharacterCard.HitPoints);
		}

		[Test]
		public void TestMrNobleLoadedStartOfGame_3Heroes()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Megalopolis");
			StartGame();
			AssertNumberOfCardsInPlay((Card c) => IsHighTierEnforcer(c), 1);
			AssertNumberOfCardsInPlay((Card c) => IsLowerTierEnforcer(c), 0);
		}

		[Test]
		public void TestMrNobleLoadedStartOfGame_4Heroes()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Megalopolis");
			StartGame();
			AssertNumberOfCardsInPlay((Card c) => IsHighTierEnforcer(c), 1);
			AssertNumberOfCardsInPlay((Card c) => IsLowerTierEnforcer(c), 1);
		}

		[Test]
		public void TestMrNobleLoadedStartOfGame_5Heroes()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			AssertNumberOfCardsInPlay((Card c) => IsHighTierEnforcer(c), 1);
			AssertNumberOfCardsInPlay((Card c) => IsLowerTierEnforcer(c), 2);
		}

		[Test]
		public void TestMrNobleStartOfTurnFlipFront_0HighTierEnforcers()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			GoToEndOfTurn(base.env);
			DestroyNonCharacterVillainCards();
			AssertNotFlipped(MrNoble);
			QuickHPStorage(base.legacy, base.haka, base.ra, base.fanatic, base.tachyon);
			GoToStartOfTurn(MrNoble);
			AssertFlipped(MrNoble);
			QuickHPCheck(-3, -3, -3, -3, -3);
		}

		[Test]
		public void TestMrNobleStartOfTurnFlipFront_1HighTierEnforcers()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			GoToEndOfTurn(base.env);
			DestroyNonCharacterVillainCards();
			AssertNotFlipped(MrNoble);
			PlayCard("LadySun");
			GoToStartOfTurn(MrNoble);
			AssertNotFlipped(MrNoble);
		}

		[Test]
		public void TestMrNobleEnforcerIncreaseFront()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			Card source = PlayCard("LadySun");
			Card source2 = PlayCard("FrostFlameBrothers");
			Card source3 = PlayCard("AndvariSoldiers");
			QuickHPStorage(base.ra);
			DealDamage(source, base.ra, 2, DamageType.Lightning);
			QuickHPCheck(-4);
			QuickHPUpdate();
			DealDamage(source2, base.ra, 2, DamageType.Lightning);
			QuickHPCheck(-4);
			QuickHPUpdate();
			DealDamage(source3, base.ra, 2, DamageType.Lightning);
			QuickHPCheck(-4);
			QuickHPUpdate();
			DealDamage(MrNoble, base.ra, 2, DamageType.Lightning);
			QuickHPCheck(-2);
		}

		[Test]
		public void TestMrNobleReduceFront()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => IsEnforcer(c)).Take(3);
			PlayCards(cardsToPlay);
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 7, DamageType.Sonic);
			QuickHPCheck(-4);
			QuickHPUpdate();
			DealDamage(base.ra, MrNoble, 7, DamageType.Sonic);
			QuickHPCheck(-7);
			GoToNextTurn();
			QuickHPUpdate();
			DealDamage(base.ra, MrNoble, 7, DamageType.Sonic);
			QuickHPCheck(-4);
		}

		[Test]
		public void TestMrNobleEndOfTurnFront()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			GoToPlayCardPhase(MrNoble);
			SetAllTargetsToMaxHP();
			Card cardToPrevent = PlayCard("TheExterminator");
			Card card = PlayCard("LadySun");
			PreventEndOfTurnEffects(MrNoble, cardToPrevent);
			PreventEndOfTurnEffects(MrNoble, card);
			QuickHPStorage(base.legacy, base.haka, base.ra, base.fanatic, base.tachyon);
			AssertNotDamageSource(card);
			AssertNotDamageSource(MrNoble.CharacterCard);
			GoToEndOfTurn(MrNoble);
			QuickHPCheck(-6, -6, 0, 0, 0);
		}

		[Test]
		public void TestMrNobleStartOfTurnFlipBack_4EnforcersInPlay()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			GoToEndOfTurn(base.env);
			FlipCard(MrNoble.CharacterCard);
			DestroyNonCharacterVillainCards();
			IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => IsEnforcer(c)).Take(4);
			PlayCards(cardsToPlay);
			AssertFlipped(MrNoble);
			QuickHPStorage(base.legacy, base.haka, base.ra, base.fanatic, base.tachyon);
			GoToStartOfTurn(MrNoble);
			AssertNotFlipped(MrNoble);
			QuickHPCheck(-1, -1, -1, -1, -1);
		}

		[Test]
		public void TestMrNobleStartOfTurnFlipBack_3EnforcersInPlay()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			GoToEndOfTurn(base.env);
			FlipCard(MrNoble.CharacterCard);
			DestroyNonCharacterVillainCards();
			IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => IsEnforcer(c)).Take(3);
			PlayCards(cardsToPlay);
			AssertFlipped(MrNoble);
			QuickHPStorage(base.legacy, base.haka, base.ra, base.fanatic, base.tachyon);
			GoToStartOfTurn(MrNoble);
			AssertFlipped(MrNoble);
			QuickHPCheckZero();
		}

		[Test]
		public void TestMrNobleIncapIncreaseBack()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			FlipCard(MrNoble.CharacterCard);
			DealDamage(MrNoble, base.legacy, 99, DamageType.Projectile);
			DealDamage(MrNoble, base.haka, 99, DamageType.Projectile);
			AssertIncapacitated(base.legacy);
			AssertIncapacitated(base.haka);
			QuickHPStorage(base.tachyon);
			DealDamage(MrNoble, base.tachyon, 4, DamageType.Projectile);
			QuickHPCheck(-6);
			Card source = PlayCard("LadySun");
			QuickHPUpdate();
			DealDamage(source, base.tachyon, 4, DamageType.Projectile);
			QuickHPCheck(-4);
		}

		[Test]
		public void TestMrNobleIncapReducesBack()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			FlipCard(MrNoble.CharacterCard);
			DealDamage(MrNoble, base.legacy, 99, DamageType.Projectile);
			DealDamage(MrNoble, base.haka, 99, DamageType.Projectile);
			AssertIncapacitated(base.legacy);
			AssertIncapacitated(base.haka);
			QuickHPStorage(MrNoble);
			DealDamage(base.tachyon, MrNoble, 4, DamageType.Projectile);
			QuickHPCheck(-2);
			Card card = PlayCard("LadySun");
			QuickHPStorage(card);
			DealDamage(base.tachyon, card, 4, DamageType.Projectile);
			QuickHPCheck(-4);
		}

		[Test]
		public void TestMrNobleEndOfTurnBack()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Haka", "Ra", "Fanatic", "Tachyon", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			SetAllTargetsToMaxHP();
			GoToPlayCardPhase(MrNoble);
			SetHitPoints(base.ra, 10);
			SetHitPoints(base.tachyon, 15);
			FlipCard(MrNoble.CharacterCard);
			Card card = PutOnDeck("TheExterminator");
			QuickHPStorage(base.legacy, base.haka, base.ra, base.fanatic, base.tachyon);
			GoToEndOfTurn(MrNoble);
			QuickHPCheck(0, -3, 0, 0, -6);
			AssertIsInPlay(card);
		}

		[Test]
		public void TestAndvariSoldiers()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "CaptainCosmic", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			SetAllTargetsToMaxHP();
			GoToPlayCardPhase(MrNoble);
			Card card = PlayCard("AndvariSoldiers");
			Card card2 = PlayCard("LadySun");
			Card card3 = PlayCard("TheGunnCouple");
			Card card4 = PlayCard("CosmicCrest");
			PreventEndOfTurnEffects(MrNoble, card2);
			PreventEndOfTurnEffects(MrNoble, card3);
			PreventEndOfTurnEffects(MrNoble, MrNoble.CharacterCard);
			QuickHPStorage(base.legacy.CharacterCard, base.ra.CharacterCard, base.haka.CharacterCard, base.cosmic.CharacterCard, card4);
			GoToEndOfTurn(MrNoble);
			QuickHPCheck(0, 0, 0, -4, 0);
			QuickHPStorage(card, card2, card3, MrNoble.CharacterCard);
			DealDamage(base.ra, card2, 1, DamageType.Melee);
			QuickHPCheck(-1, 0, 0, 0);
			SetAllTargetsToMaxHP();
			QuickHPUpdate();
			DealDamage(base.ra, MrNoble, 4, DamageType.Melee);
			QuickHPCheck(-4, 0, 0, 0);
			SetAllTargetsToMaxHP();
			QuickHPUpdate();
			DealDamage(base.ra, card3, 1, DamageType.Melee);
			QuickHPCheck(0, 0, -1, 0);
		}

		[Test]
		public void TestAssetRecovery()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "CaptainCosmic", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			QuickShuffleStorage(MrNoble);
			PlayCard("AssetRecovery");
			QuickShuffleCheck(2);
			AssertNumberOfCardsInTrash(MrNoble, 1);
			AssertNumberOfCardsInPlay((Card c) => IsLowerTierEnforcer(c), 1);
			AssertNumberOfCardsInRevealed(MrNoble, 0);
		}

		[Test]
		public void TestBrutalBusinessPractices()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "CaptainCosmic", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			GoToPlayCardPhase(MrNoble);
			Card card = PlayCard("AndvariSoldiers");
			PlayCard("BrutalBusinessPractices");
			Card card2 = PlayCard("PlummetingMonorail");
			QuickHPStorage(card);
			DealDamage(base.legacy, card, 2, DamageType.Melee);
			QuickHPCheck(-1);
			QuickHPStorage(card2);
			DealDamage(base.legacy, card2, 1, DamageType.Melee);
			QuickHPCheck(-4);
		}

		[Test]
		public void TestFrostFlameBrothers()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			GoToPlayCardPhase(MrNoble);
			Card card = PlayCard("FrostFlameBrothers");
			QuickHPStorage(MrNoble.CharacterCard, card, base.legacy.CharacterCard, base.ra.CharacterCard, base.haka.CharacterCard);
			DealDamage(base.legacy, card, 3, DamageType.Melee);
			QuickHPCheck(0, -2, -6, -6, -6);
			QuickHPUpdate();
			DealDamage(base.legacy, card, 3, DamageType.Melee);
			QuickHPCheck(0, -2, 0, 0, 0);
			GoToNextTurn();
			QuickHPUpdate();
			DealDamage(base.legacy, card, 3, DamageType.Melee);
			QuickHPCheck(0, -2, -6, -6, -6);
		}

		[Test]
		public void TestIntimidatingPrecence()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			GoToPlayCardPhase(MrNoble);
			Card card = PlayCard("NextEvolution");
			Card card2 = PlayCard("ImbuedFire");
			Card card3 = PlayCard("FlameBarrier");
			Card card4 = PlayCard("Dominion");
			Card card5 = PlayCard("BrutalBusinessPractices");
			AssertIsInPlay(card, card2, card3, card4, card5);
			Card card6 = PutOnDeck("AndvariSoldiers");
			PlayCard("IntimidatingPresence");
			AssertInTrash(card, card2, card3, card4, card5);
			AssertInPlayArea(MrNoble, card6);
		}

		[Test]
		public void TestLadySunStartOfTurn()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			GoToEndOfTurn(base.env);
			DestroyNonCharacterVillainCards();
			PlayCard("LadySun");
			base.DecisionSelectCards = new Card[4]
			{
			base.legacy.HeroTurnTaker.Hand.TopCard,
			base.ra.HeroTurnTaker.Hand.TopCard,
			null,
			null
			};
			QuickHandStorage(base.legacy, base.ra, base.haka);
			QuickHPStorage(base.legacy, base.ra, base.haka);
			GoToStartOfTurn(MrNoble);
			QuickHandCheck(-1, -1, 0);
			QuickHPCheck(0, 0, -5);
		}

		[Test]
		public void TestLandTakeover_DestroysTarget()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			Card card = PlayCard("PlummetingMonorail");
			Card card2 = PlayCard("TrafficPileup");
			PutOnDeck("LeadershipOfGreed");
			SetHitPoints(card2, 3);
			AssertInPlayArea(base.env, card);
			AssertInPlayArea(base.env, card2);
			QuickHPStorage(base.legacy, base.ra, base.haka);
			PlayCard("LandTakeover");
			AssertInTrash(card2);
			AssertInPlayArea(base.env, card);
			QuickHPCheck(-2, -2, -2);
		}

		[Test]
		public void TestLandTakeover_NoDestroyTarget()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			IEnumerable<Card> enumerable = FindCardsWhere((Card c) => base.env.TurnTaker.Deck.HasCard(c) && !c.IsTarget).Take(5);
			PlayCards(enumerable);
			QuickHPStorage(base.legacy, base.ra, base.haka);
			PlayCard("LandTakeover");
			AssertInTrash(enumerable);
			QuickHPCheck(0, 0, 0);
		}

		[Test]
		public void TestLawsuits()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			GoToPlayCardPhase(MrNoble);
			Card card = PlayCard("TheLegacyRing");
			Card card2 = PlayCard("TheStaffOfRa");
			Card card3 = PlayCard("Mere");
			Card card4 = PlayCard("Taiaha");
			AssertIsInPlay(card, card2, card4, card3);
			Card card5 = PutOnDeck("AndvariSoldiers");
			PlayCard("Lawsuits");
			AssertInTrash(card, card2, card4, card3);
			AssertInPlayArea(MrNoble, card5);
		}

		[Test]
		public void TestLeadershipOfGreed()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			GoToPlayCardPhase(MrNoble);
			Card card = PlayCard("LadySun");
			PlayCard("LeadershipOfGreed");
			QuickHPStorage(base.ra);
			DealDamage(card, base.ra, 3, DamageType.Radiant);
			QuickHPCheck(-7);
			QuickHPStorage(card);
			DealDamage(base.ra, card, 3, DamageType.Fire);
			QuickHPCheck(-3);
		}

		[Test]
		public void TestLadySunImmunity()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			PlayCard("LadySun");
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Infernal);
			QuickHPCheckZero();
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Radiant);
			QuickHPCheckZero();
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Melee, isIrreducible: true);
			QuickHPCheck(-5);
		}

		[Test]
		public void TestOneShotImmunity()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			PlayCard("OneShot");
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Melee);
			QuickHPCheckZero();
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Projectile);
			QuickHPCheckZero();
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Radiant, isIrreducible: true);
			QuickHPCheck(-5);
		}

		[Test]
		public void TestRecruitmentDrive()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			PutOnDeck("LeadershipOfGreed");
			IEnumerable<Card> deck = FindCardsWhere((Card c) => IsLowerTierEnforcer(c)).Take(3);
			PutOnDeck(MrNoble, deck);
			IEnumerable<Card> cards = FindCardsWhere((Card c) => IsLowerTierEnforcer(c) && !deck.Contains(c));
			PutInTrash(MrNoble, cards);
			int numberOfCardsInTrash = GetNumberOfCardsInTrash(MrNoble);
			PlayCard("RecruitmentDrive");
			AssertInPlayArea(MrNoble, deck);
			AssertNumberOfCardsInTrash(MrNoble, numberOfCardsInTrash + 2);
		}

		[Test]
		public void TestOneShot_OnCardPlay()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			Card card = PutInTrash("LeadershipOfGreed");
			Card card2 = PutOnDeck("BrutalBusinessPractices");
			PlayCard("OneShot");
			QuickHPStorage(base.legacy, base.ra, base.haka);
			PlayCard(card2);
			QuickHPCheck(-6, 0, 0);
			SetAllTargetsToMaxHP();
			QuickHPUpdate();
			PlayCard(card);
			QuickHPCheckZero();
		}

		[Test]
		public void TestTheExterminatorImmunity()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			SetAllTargetsToMaxHP();
			PlayCard("TheExterminator");
			Card source = PlayCard("TrafficPileup");
			QuickHPStorage(MrNoble);
			DealDamage(source, MrNoble, 5, DamageType.Psychic, isIrreducible: true);
			QuickHPCheckZero();
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Radiant, isIrreducible: true);
			QuickHPCheck(-5);
		}

		[Test]
		public void TestTheExterminatorEndOfTurn()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			SetAllTargetsToMaxHP();
			Card card = PlayCard("TheExterminator");
			PreventEndOfTurnEffects(MrNoble, MrNoble.CharacterCard);
			QuickHPStorage(base.legacy, base.ra, base.haka);
			GoToEndOfTurn(MrNoble);
			QuickHPCheck(0, 0, -5);
		}

		[Test]
		public void TestTheGunnCouple()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			SetAllTargetsToMaxHP();
			GoToPlayCardPhase(MrNoble);
			Card source = PlayCard("TheGunnCouple");
			Card card = PlayCard("AndvariSoldiers");
			QuickHPStorage(base.ra);
			DealDamage(source, base.ra, 3, DamageType.Projectile);
			QuickHPCheck(-5);
			QuickHPUpdate();
			DealDamage(card, base.ra, 3, DamageType.Projectile);
			QuickHPCheck(-6);
			PreventEndOfTurnEffects(MrNoble, MrNoble.CharacterCard);
			PreventEndOfTurnEffects(MrNoble, card);
			SetAllTargetsToMaxHP();
			QuickHPStorage(base.legacy, base.ra, base.haka);
			GoToEndOfTurn(MrNoble);
			QuickHPCheck(0, 0, -4);
			GoToEndOfTurn(base.env);
			PreventStartOfTurnEffects(MrNoble, MrNoble.CharacterCard);
			SetAllTargetsToMaxHP();
			QuickHPUpdate();
			GoToStartOfTurn(MrNoble);
			QuickHPCheck(0, 0, -4);
		}

		[Test]
		public void TestTheLivingShadowImmunity()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			PlayCard("TheLivingShadow");
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Psychic);
			QuickHPCheckZero();
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Energy);
			QuickHPCheckZero();
			QuickHPStorage(MrNoble);
			DealDamage(base.ra, MrNoble, 5, DamageType.Radiant, isIrreducible: true);
			QuickHPCheck(-5);
		}

		[Test]
		public void TestTheLivingShadowStartOfTurn_CardsInPlay()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			PlayCard("TheLivingShadow");
			GoToEndOfTurn(base.env);
			SetAllTargetsToMaxHP();
			Card card = PlayCard("SurgeOfStrength");
			Card card2 = PlayCard("FlameBarrier");
			Card card3 = PlayCard("Dominion");
			Card card4 = PlayCard("Mere");
			base.DecisionSelectCard = card3;
			GoToStartOfTurn(MrNoble);
			AssertInTrash(card3);
			AssertInPlayArea(base.legacy, card);
			AssertInPlayArea(base.ra, card2);
			AssertInPlayArea(base.haka, card4);
		}

		[Test]
		public void TestTheLivingShadowStartOfTurn_NoCardsInPlay()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			PlayCard("TheLivingShadow");
			GoToEndOfTurn(base.env);
			SetAllTargetsToMaxHP();
			GoToStartOfTurn(MrNoble);
			AssertNotGameOver();
		}

		[Test]
		public void TestTrueColors()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			SetAllTargetsToMaxHP();
			SetHitPoints(base.legacy, 10);
			SetHitPoints(base.ra, 11);
			IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => IsLowerTierEnforcer(c)).Take(3);
			PlayCards(cardsToPlay);
			QuickHPStorage(base.legacy, base.ra, base.haka);
			PlayCard("TrueColors");
			QuickHPCheck(0, 0, -9);
		}

		[Test]
		public void TestTrueColors_NoneInPlay()
		{
			SetupGameController("TheDragonRune.MrNoble", "Legacy", "Ra", "Haka", "Megalopolis");
			StartGame();
			DestroyNonCharacterVillainCards();
			SetAllTargetsToMaxHP();
			SetHitPoints(base.legacy, 10);
			SetHitPoints(base.ra, 11);
			QuickHPStorage(base.legacy, base.ra, base.haka);
			PlayCard("TrueColors");
			QuickHPCheck(0, 0, 0);
		}
	}

}