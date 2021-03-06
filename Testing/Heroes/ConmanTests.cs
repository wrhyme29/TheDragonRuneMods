﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using TheDragonRune.Conman;

namespace TheDragonRuneTest
{

    [TestFixture()]
    public class ConmanTests : CustomBaseTest
    {

        #region ConmanHelperFunctions
        public static readonly string ChangelingKeyword = "changeling";
        public static readonly string IllusionKeyword = "illusion";
        public static readonly string TeleportKeyword = "teleport";

        protected bool IsChangeling(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, ChangelingKeyword);
        }
        protected bool IsTeleport(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, TeleportKeyword);
        }
        protected bool IsIllusion(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, IllusionKeyword);
        }

        #endregion

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(conman.CharacterCard, 1);
            DealDamage(villain, conman, 2, DamageType.Melee, true);
        }

        [Test()]
        public void TestLoadConman()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(conman);
            Assert.IsInstanceOf(typeof(ConmanCharacterCardController), conman.CharacterCardController);

            Assert.AreEqual(31, conman.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestConmanInnatePower()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //The next time {Conman} would take Damage, redirect it to another  hero target and reduce the damage by 2.
            UsePower(conman.CharacterCard);
            SelectCardsForNextDecision(bunker.CharacterCard);
            QuickHPStorage(baron, conman, legacy, bunker, scholar);
            DealDamage(baron, conman, 5, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -3, 0);

            //only next damage
            QuickHPUpdate();
            DealDamage(baron, conman, 5, DamageType.Melee);
            QuickHPCheck(0, -5, 0, 0, 0);

        }

        [Test()]
        public void TestConmanIncap3()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);
            //Select a hero target. The next time that target would be dealt damage, redirect that damage to another target.
            DecisionSelectCards = new Card[] { bunker.CharacterCard, bunker.CharacterCard, baron.CharacterCard };
            UseIncapacitatedAbility(conman, 2);
            //check for no stacking
            UseIncapacitatedAbility(conman, 2);

            QuickHPStorage(baron, legacy, bunker, scholar);
            DealDamage(baron, bunker, 5, DamageType.Fire);
            QuickHPCheck(-5, 0, 0, 0);
            //next only
            QuickHPUpdate();
            DealDamage(baron, bunker, 5, DamageType.Fire);
            QuickHPCheck(0, 0, -5, 0);
        }

        [Test()]
        public void TestEarthIllusion_PlayCard()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card earth = PlayCard("EarthIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, earth);
            QuickShuffleCheck(1);

            //{Conman} may not Play card or use powers.
            Card handCard = GetRandomCardFromHand(conman);
            AssertCannotPlayCards(conman);
            GoToUsePowerPhase(conman);
            AssertCannotPerformPhaseAction();

            //At the end of {Conman}’s turn, one other player may play a card or use a power.
            DecisionSelectTurnTaker = bunker.TurnTaker;
            Card playCard = PutInHand("AmmoDrop");
            DecisionSelectCard = playCard;
            DecisionSelectFunction = 0;
            GoToEndOfTurn(conman);
            AssertInPlayArea(bunker, playCard);

            //At the start of {Conman}’s turn, you may shuffle this card into the deck.
            DecisionYesNo = true;
            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToStartOfTurn(conman);
            AssertInDeck(earth);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestEarthIllusion_UsePower()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card earth = PlayCard("EarthIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, earth);
            QuickShuffleCheck(1);

            //{Conman} may not Play card or use powers.
            Card handCard = GetRandomCardFromHand(conman);
            AssertCannotPlayCards(conman);
            GoToUsePowerPhase(conman);
            AssertCannotPerformPhaseAction();

            //At the end of {Conman}’s turn, one other player may play a card or use a power.
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectFunction = 1;
            QuickHandStorage(bunker);
            GoToEndOfTurn(conman);
            QuickHandCheck(1);

            //At the start of {Conman}’s turn, you may shuffle this card into the deck.
            DecisionYesNo = true;
            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToStartOfTurn(conman);
            AssertInDeck(earth);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestEarthIllusion_Optional()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card earth = PlayCard("EarthIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, earth);
            QuickShuffleCheck(1);

            //{Conman} may not Play card or use powers.
            Card handCard = GetRandomCardFromHand(conman);
            AssertCannotPlayCards(conman);
            GoToUsePowerPhase(conman);
            AssertCannotPerformPhaseAction();

            //At the end of {Conman}’s turn, one other player may play a card or use a power.
            DecisionSelectTurnTaker = bunker.TurnTaker;
            Card playCard = PutInHand("AmmoDrop");
            DecisionSelectCard = playCard;
            DecisionDoNotSelectFunction = true;
            QuickHandStorage(bunker);
            GoToEndOfTurn(conman);
            QuickHandCheckZero();
            AssertInHand(playCard);

            //At the start of {Conman}’s turn, you may shuffle this card into the deck.
            DecisionYesNo = true;
            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToStartOfTurn(conman);
            AssertInDeck(earth);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestLastingIllusion_DrawCard()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card last = PlayCard("LastingIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, last);
            QuickShuffleCheck(1);
            //During {Conman}’s draw phase, one other hero may also draw a card.
            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(conman, legacy, bunker, scholar);
            GoToDrawCardPhase(conman);
            QuickHandCheck(0, 1, 0, 0);

        }

        [Test()]
        public void TestLastingIllusion_PreventShuffle()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card last = PlayCard("LastingIllusion");
            AssertInPlayArea(conman, last);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card changeling = PutInDeck("WindIllusion");
            DecisionSelectCard = GetRandomCardFromHand(conman);
            PlayCard(changeling);
            AssertInPlayArea(conman, changeling);
            AssertInPlayArea(conman, last);
            QuickShuffleCheck(0);

        }

        [Test()]
        public void TestRavagerIllusion()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card ravage = PlayCard("RavagerIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, ravage);
            QuickShuffleCheck(1);
            //Increase damage dealt by Hero targets by 1, and reduce damage dealt to {Conman} by 1.
            QuickHPStorage(baron);
            DealDamage(conman, baron, 2, DamageType.Fire);
            QuickHPCheck(-3);
            QuickHPUpdate();
            DealDamage(legacy, baron, 2, DamageType.Fire);
            QuickHPCheck(-4);
            QuickHPUpdate();
            DealDamage(bunker, baron, 2, DamageType.Fire);
            QuickHPCheck(-3);
            QuickHPUpdate();
            DealDamage(scholar, baron, 2, DamageType.Fire);
            QuickHPCheck(-3);
            QuickHPStorage(conman, legacy, bunker, scholar);
            DealDamage(baron, (Card c) => c.IsHero, 2, DamageType.Fire);
            QuickHPCheck(-1,-3,-2,-2);

        }
        [Test()]
        public void TestShadowedIllusion()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(conman);
            Card changeling = PlayCard("WindIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card shadow = PlayCard("ShadowedIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, shadow);
            QuickShuffleCheck(1);
            //Reduce damage dealt to hero targets by 2 and increase damage dealt by {Conman} by 1.
            QuickHPStorage(baron);
            DealDamage(conman, baron, 2, DamageType.Fire);
            QuickHPCheck(-3);
            QuickHPStorage(conman, legacy, bunker, scholar);
            DealDamage(baron, (Card c) => c.IsHero, 3, DamageType.Fire);
            QuickHPCheck(-1, -2, -1, -1);

        }

        [Test()]
        public void TestWindIllusion()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToStartOfTurn(conman);
            Card changeling = PlayCard("ShadowedIllusion");
            AssertInPlayArea(conman, changeling);
            //When this card enters play shuffle all other Changeling cards into {Conman}’s deck."
            QuickShuffleStorage(conman.TurnTaker.Deck);
            Card wind = PlayCard("WindIllusion");
            AssertInDeck(changeling);
            AssertInPlayArea(conman, wind);
            QuickShuffleCheck(1);
            //{Conman} may play another card and use an additional power on his turn.
            GoToPlayCardPhase(conman);
            AssertPhaseActionCount(2);
            GoToUsePowerPhase(conman);
            AssertPhaseActionCount(2);

        }
        [Test()]
        public void TestIllusionAndFusion_Villain()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            //Shuffle a Non-Character Card target back into their respective deck.
            //If that target in a Villan play area, you may draw a card.
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card turret = PlayCard("RegressionTurret");
            DecisionSelectCard = mdp;
            QuickHandStorage(conman);
            QuickShuffleStorage(baron.TurnTaker.Deck);
            PlayCard("IllusionAndFusion");
            AssertInDeck(mdp);
            QuickShuffleCheck(1);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestIllusionAndFusion_Hero()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Luminary", "TheScholar", "Megalopolis");
            StartGame();
            //Shuffle a Non-Character Card target back into their respective deck.
            //If that target in a Villan play area, you may draw a card.

            Card turret = PlayCard("RegressionTurret");
            Card hand = PutInHand("BacklashGenerator");
            DecisionSelectCards = new Card[] { turret, hand };
            QuickShuffleStorage(luminary.TurnTaker.Deck);
            PlayCard("IllusionAndFusion");
            QuickShuffleCheck(1);
            AssertInDeck(turret);
            AssertInPlayArea(luminary, hand);
            
        }

        [Test()]
        public void TestIllusionAndFusion_Environment()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Luminary", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Shuffle a Non-Character Card target back into their respective deck.
            //If that target was in an Environment play area, you may use a power now.
            Card envTarget = PlayCard("TargetingInnocents");
            Card turret = PlayCard("RegressionTurret");
            DecisionSelectCards = new Card[] { envTarget };
            QuickShuffleStorage(env.TurnTaker.Deck);
            PlayCard("IllusionAndFusion");
            QuickShuffleCheck(1);
            AssertInDeck(envTarget);

            SelectCardsForNextDecision(bunker.CharacterCard);
            QuickHPStorage(baron, conman, legacy, bunker, luminary);
            DealDamage(baron, conman, 5, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -3, 0);

            //only next damage
            QuickHPUpdate();
            DealDamage(baron, conman, 5, DamageType.Melee);
            QuickHPCheck(0, -5, 0, 0, 0);

        }

        [Test()]
        public void TestIMisleadYou()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Deal 2 psychic damage to a target. Reduce the next damage they would deal by 1.
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("IMisleadYou");
            QuickHPCheck(-2);
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 2, DamageType.Fire);
            QuickHPCheck(-1);
            QuickHPUpdate();
            DealDamage(baron, bunker, 2, DamageType.Fire);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestImOutOfHere()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(conman);
            //When this card enters play, {Conman} deals himself 5 psychic damage. 
            QuickHPStorage(conman);
            Card card = GetCard("ImOutOfHere");
            IEnumerable<Card> putInTrash = FindCardsWhere(c => conman.TurnTaker.Deck.HasCard(c) && c != card).Take(5);
            PutInTrash(putInTrash);
            PlayCard(card);
            QuickHPCheck(-5);
            //If {Conman} takes damage that way, the first time a turn {Conman} would take damage redirect it to the Villain Target with the highest hp and increase the damage by 1.",
            //At the end of {Conman}’s next turn, destroy this card and shuffle it and {Conman}'s trash into {Conman}’s deck."
            AssertNumberOfCardsInTrash(conman, 5);
            GoToEndOfTurn(conman);
            AssertInPlayArea(conman, card);
            AssertNumberOfCardsInTrash(conman, 5);

            GoToNextTurn();
            QuickHPStorage(baron, conman, legacy, bunker, luminary);
            DealDamage(bunker, conman, 5, DamageType.Fire);
            QuickHPCheck(-6, 0, 0, 0, 0);

            //only first damage
            QuickHPUpdate();
            DealDamage(bunker, conman, 5, DamageType.Fire);
            QuickHPCheck(0, -5, 0, 0, 0);

            //resets at next turn
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(bunker, conman, 5, DamageType.Fire);
            QuickHPCheck(-6, 0, 0, 0, 0);

            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToEndOfTurn(conman);
            AssertNumberOfCardsInTrash(conman, 0);
            AssertInDeck(putInTrash);
            AssertInDeck(card);
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestImOutOfHere_NoInitialDamage()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(conman);
            //When this card enters play, {Conman} deals himself 5 psychic damage. 
            QuickHPStorage(conman);
            Card card = GetCard("ImOutOfHere");
            IEnumerable<Card> putInTrash = FindCardsWhere(c => conman.TurnTaker.Deck.HasCard(c) && c != card).Take(5);
            PutInTrash(putInTrash);
            AddImmuneToNextDamageEffect(conman, false, true);
            PlayCard(card);
            QuickHPCheck(0);
            //If {Conman} takes damage that way, the first time a turn {Conman} would take damage redirect it to the Villain Target with the highest hp and increase the damage by 1.",
            //At the end of {Conman}’s next turn, destroy this card and shuffle it and {Conman}'s trash into {Conman}’s deck."
            AssertNumberOfCardsInTrash(conman, 5);
            GoToEndOfTurn(conman);
            AssertInPlayArea(conman, card);
            AssertNumberOfCardsInTrash(conman, 5);

            GoToNextTurn();
            QuickHPStorage(baron, conman, legacy, bunker, luminary);
            DealDamage(bunker, conman, 5, DamageType.Fire);
            QuickHPCheck(0, -5, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(bunker, conman, 5, DamageType.Fire);
            QuickHPCheck(0, -5, 0, 0, 0);

            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(bunker, conman, 5, DamageType.Fire);
            QuickHPCheck(0, -5, 0, 0, 0);

            QuickShuffleStorage(conman.TurnTaker.Deck);
            GoToEndOfTurn(conman);
            AssertNumberOfCardsInTrash(conman, 0);
            AssertInDeck(putInTrash);
            AssertInDeck(card);
            QuickShuffleCheck(1);
        }
        [Test()]
        public void TestInspiringAttack()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(new Card[] { conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard }, 10);
            //Deal 1 psychic damage to a villain target. Each other hero targets gains 2 hp.
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron, conman, legacy, bunker, luminary);
            PlayCard("InspiringAttack");
            QuickHPCheck(-1, 0, 2, 2, 2);
        }

        [Test()]
        public void TestMoreOfMe()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //"If there is a Changeling card in play, {Conman} may use an extra power on his turn.",
            Card changeling = PlayCard("ShadowedIllusion");
            Card more = PlayCard("MoreOfMe");
            GoToUsePowerPhase(conman);
            AssertPhaseActionCount(2);

            //"Whenever a Changeling card would be destroyed, you may destroy this card instead."
            DecisionYesNo = true;
            DestroyCard(changeling);
            AssertInTrash(more);
            AssertInPlayArea(conman, changeling);
        }
        [Test()]
        public void TestMoreOfMe_OptionalDestroy()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //"If there is a Changeling card in play, {Conman} may use an extra power on his turn.",
            Card changeling = PlayCard("ShadowedIllusion");
            Card more = PlayCard("MoreOfMe");
            GoToUsePowerPhase(conman);
            AssertPhaseActionCount(2);

            //"Whenever a Changeling card would be destroyed, you may destroy this card instead."
            DecisionYesNo = false;
            DestroyCard(changeling);
            AssertInTrash(changeling);
            AssertInPlayArea(conman, more);
        }
        [Test()]
        public void TestMyStuffNow()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");

            IEnumerable<Card> putInTrash = FindCardsWhere(c => conman.TurnTaker.Deck.HasCard(c) && (IsIllusion(c) || IsTeleport(c))).Take(2);
            PutInTrash(putInTrash);
            //Choose a Villain target, reduce all damage they would deal by X until the start of {Conman}’s turn, where X = the number of Illusions and teleports in {Conman}’s Trash.
            DecisionSelectCard = baron.CharacterCard;
            PlayCard("MyStuffNow");
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-1);

            //not just next damage
            QuickHPUpdate();
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-1);

            //only that villain target
            QuickHPUpdate();
            DealDamage(battalion, bunker, 3, DamageType.Fire);
            QuickHPCheck(-3);

            //expires
            GoToStartOfTurn(conman);
            QuickHPUpdate();
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestMyTimeToShine_13Cards()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            ShuffleTrashIntoDeck(baron);
            SetHitPoints(conman, 10);
            Card myTime = GetCard("MyTimeToShine");
            IEnumerable<Card> toTrash = FindCardsWhere(c => c.Location.IsDeck && c.IsHero && c != myTime).Take(13);
            PutInTrash(toTrash);
            //Shuffle all cards in each trash into their respective decks.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion, bunker.CharacterCard, luminary.CharacterCard };
            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            PlayCard(myTime);
            //For every 3 cards shuffled from the trash this way, {Conman} may deal up to that many targets 2 melee damage, and 1 psychic damage.
            //Gain X hp, where X is the number of cards shuffled this way / 5
            //4 targets, 2 HP
            QuickHPCheck(-3, -3, 2, 0, -3, -3);
            //If at least 40 cards are shuffled this way, return this card to your hand instead of it going into the trash
            //not true
            AssertInTrash(myTime);
            AssertNumberOfCardsInTrash(baron, 0);
            AssertNumberOfCardsInTrash(conman, 1);
            AssertNumberOfCardsInTrash(legacy, 0);
            AssertNumberOfCardsInTrash(bunker, 0);
            AssertNumberOfCardsInTrash(luminary, 0);
            AssertNumberOfCardsInTrash(env, 0);

        }

        [Test()]
        public void TestMyTimeToShine_40Cards()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            ShuffleTrashIntoDeck(baron);
            SetHitPoints(conman, 10);
            Card myTime = GetCard("MyTimeToShine");
            IEnumerable<Card> toTrash = FindCardsWhere(c => c.Location.IsDeck && c.IsHero && c != myTime).Take(43);
            PutInTrash(toTrash);
            //Shuffle all cards in each trash into their respective decks.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion, bunker.CharacterCard, luminary.CharacterCard, legacy.CharacterCard, null };
            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            PlayCard(myTime);
            //For every 3 cards shuffled from the trash this way, {Conman} may deal up to that many targets 2 melee damage, and 1 psychic damage.
            //Gain X hp, where X is the number of cards shuffled this way / 5
            //14 targets, 8 HP
            QuickHPCheck(-3, -3, 8, -3, -3, -3);
            //If at least 40 cards are shuffled this way, return this card to your hand instead of it going into the trash
            //not true
            AssertInHand(myTime);
            AssertNumberOfCardsInTrash(baron, 0);
            AssertNumberOfCardsInTrash(conman, 0);
            AssertNumberOfCardsInTrash(legacy, 0);
            AssertNumberOfCardsInTrash(bunker, 0);
            AssertNumberOfCardsInTrash(luminary, 0);
            AssertNumberOfCardsInTrash(env, 0);

        }

        [Test()]
        public void TestNeverToldYouALie()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //"Deal 2 targets 2 psychic damage. Those targets cannot deal damage until {Conman}'s next turn.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            PlayCard("NeverToldYouALie");
            QuickHPCheck(-2, -2, 0, 0, 0, 0);

            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 5, DamageType.Melee);
            QuickHPCheckZero();

            QuickHPUpdate();
            DealDamage(battalion, bunker, 5, DamageType.Melee);
            QuickHPCheckZero();

            //expires
            GoToStartOfTurn(conman);
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 5, DamageType.Melee);
            QuickHPCheck(-5);

            QuickHPUpdate();
            DealDamage(battalion, bunker, 5, DamageType.Melee);
            QuickHPCheck(-5);


        }

        [Test()]
        public void TestNeverToldYouALie_EvenWhenImmune()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //"Deal 2 targets 2 psychic damage. Those targets cannot deal damage until {Conman}'s next turn.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            AddImmuneToDamageTrigger(baron, false, true);
            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            PlayCard("NeverToldYouALie");
            QuickHPCheck(0,0, 0, 0, 0, 0);

            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 5, DamageType.Melee);
            QuickHPCheckZero();

            QuickHPUpdate();
            DealDamage(battalion, bunker, 5, DamageType.Melee);
            QuickHPCheckZero();

            //expires
            GoToStartOfTurn(conman);
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 5, DamageType.Melee);
            QuickHPCheck(-5);

            QuickHPUpdate();
            DealDamage(battalion, bunker, 5, DamageType.Melee);
            QuickHPCheck(-5);


        }

        [Test()]
        public void TestSwitcherooStrikes()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            PlayCard("TeleportSpam");
            IEnumerable<Card> toTrash = FindCardsWhere(c => conman.TurnTaker.Deck.HasCard(c) && IsTeleport(c)).Take(2);
            PutInTrash(toTrash);
            // Deal X targets 1 melee and 3 psychic damage, where X is the number of teleports in play and in { Conman}’s Trash."
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion, luminary.CharacterCard };
            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            PlayCard("SwitcherooStrikes");
            QuickHPCheck(-4, -4, 0, 0, 0, -4);


        }

        [Test()]
        public void TestTeleportingPunch ()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //Deal 2 Melee Damage to 2 different Targets.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            PlayCard("TeleportingPunch");
            QuickHPCheck(-2, -2, 0, 0, 0, 0);


        }

        [Test()]
        public void TestTeleportSpam()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //The first time each turn {Conman} would take damage, redirect it to a Villain Target.
            PlayCard("TeleportSpam");
            DecisionSelectCard = baron.CharacterCard;

            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            DealDamage(battalion, conman, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, 0);

            //only first damage per turn
            QuickHPUpdate();
            DealDamage(battalion, conman, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0, 0, 0);

            //resets next turn
            GoToNextTurn();

            //only conman
            QuickHPUpdate();
            DealDamage(battalion, legacy, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -2, 0, 0);

            QuickHPUpdate();
            DealDamage(battalion, conman, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, 0);

        }

        [Test()]
        public void TestTOOCloseToHome()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //Reduce the first damage dealt each turn by a Villain target by 1.
            PlayCard("TooCloseToHome");
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 4, DamageType.Fire);
            QuickHPCheck(-3);

            //first damage only
            QuickHPUpdate();
            DealDamage(baron, bunker, 4, DamageType.Fire);
            QuickHPCheck(-4);

            //resets next turn
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(battalion, bunker, 4, DamageType.Fire);
            QuickHPCheck(-3);

            //across all villain targets
            QuickHPUpdate();
            DealDamage(baron, bunker, 4, DamageType.Fire);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestWhosThatNextToYou()
        {
            SetupGameController("BaronBlade", "TheDragonRune.Conman", "Legacy", "Bunker", "Luminary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //One Villain Target deals 2 melee damage to another target.
            DecisionSelectCards = new Card[] { baron.CharacterCard, legacy.CharacterCard };
            QuickHPStorage(baron.CharacterCard, battalion, conman.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, luminary.CharacterCard);
            PlayCard("WhosThatNextToYou");
            QuickHPCheck(0, 0, 0, -3, 0, 0);
        }


    }
}
