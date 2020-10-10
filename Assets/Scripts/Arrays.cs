﻿/// <summary>
/// Contains all the static strings used in FTC.
/// </summary>
static class Arrays
{
	public const int EditorMaxStage = 3;
	public const string Version = "v1.1";

	public static readonly int[,] ColorTable = new int[3,8]
	{
		{ 1, 6, 8, 5, 2, 7, 3, 4 },
		{ 7, 2, 5, 4, 6, 3, 1, 8 },
		{ 3, 8, 1, 6, 4, 5, 7, 2 }
	};

    public static readonly string[] ColorLog =
    {
        "Red",
		"Orange",
		"Yellow",
		"Green",
		"Cyan",
		"Blue",
		"Purple",
		"White",
		"Pink",
		"Maroon",
		"Gray"
    };

    public static readonly string[] DebugText =
    {
        "largeH",
        "largeD",
        "large",
        "cylA",
        "cylB",
        "cylC",
        "gear#",
        "gearC",
        "nixieL",
        "nixieR",
        "stage",
        "quit"
    };

    public static string[] Ignore = 
    {
		"14", 
		"42", 
		"501", 
		"A>N<D", 
		"Bamboozling Time Keeper",
		"Brainf---", 
		"Busy Beaver", 
		"Don't Touch Anything", 
		"Forget Enigma", 
		"Forget Everything", 
		"Forget It Not", 
		"Forget Me Not", 
		"Forget Me Later", 
		"Forget Perspective", 
		"Forget The Colors", 
		"Forget Them All", 
		"Forget This", 
		"Forget Us Not", 
		"Iconic", 
		"Kugelblitz", 
		"Multitask", 
		"OmegaForget", 
		"Organization", 
		"Password Destroyer", 
		"Purgatory", 
		"RPS Judging", 
		"Simon Forgets", 
		"Simon's Stages", 
		"Souvenir", 
		"Tallordered Keys", 
		"The Time Keeper", 
		"The Troll", 
		"The Twin", 
		"The Very Annoying Button", 
		"Timing Is Everything", 
		"Turn The Key", 
		"Ultimate Custom Night", 
		"Übermodule", 
		"Whiteout"
    };

    public static readonly string[] FailPhrases = 
    {
        "You did a goodn't",
		"Congratulations! You got a strike",
		"You have just won a free gift card containing 1 strike and no solve! In other words",
		"This is so sad",
		"This must be really embarrasing for you",
		"I just came back, where we again? Oh yeah",
		"Unsuprisingly, your 1/91 chance went unsuccessful",
		"Did Emik break the module or are you just bad?",
		"Did Cooldoom break the module or are you just bad?",
		"This looks like a WHITE ABORT to me",
		"Correct... your mistakes in the future",
		"?!",
		"‽",
		"The phrase \"It's just a module\" is such a weak mindset, you are okay with what happened, striking, imperfection of a solve",
		"Good for you",
		"Have fun doing the math again",
		"Was that MAROON or RED?",
		"Are you sure the experts wrote it down correctly?",
		"Are you sure the defuser said it correctly?",
		"The key spun backwards",
		"THE ANSWER IS IN THE WRONG POSITION",
		"key.wav",
		"Module.HandleStrike()",
		"Is your calculator broken?",
		"Is your KTANE broken?",
		"A wide-screen monitor would really help here",
		"VR would make this easier",
		"A mechanical keyboard would make this easier",
		"A \"gaming mouse\" would make this easier",
		"E",
		"bruh moment",
		"Failed executing external process for 'Bake Runtime' job",
		"Did Discord cut vital communication off?",
		"You failed the vibe check",
		"Looks like you failed your exam",
        "Could not find USER-ANSWER in ACTUAL-ANSWER",
		"nah",
		"noppen",
		"yesn't",
		"This is the moment where you quit out the bomb",
		"You just lost the game",
		"Noooo, why'd you do that?",
		"*pufferfish noises*",
		"I was thinking about being generous this round, it didn't change my mind though",
		"Have you tried turning this module on and off?",
		"It's been so long, since I last have seen an answer, lost to this monster",
		"Oof",
		"Yikes",
		"Good luck figuring out why you're wrong",
		"Oog",
		"Nice one buckaroo",
		":̶.̶|̶:̶;̶  <--- Is this loss?",
		"Oh, you got it wrong? Report it as a bug because it's definitely not your fault",
		"I'm not rated \"Very Hard\" for no reason after all",
		"Forget The Colors be like: cringe",
		"The manual said I is Pink, are you colorblind?",
		"Not cool, meet your doom",
		"What were you thinking!?",
		"Emmm, ik you messed up somewhere",
		"You should double check that part where you messed up",
		"Looks like the expert chose betray",
		"At least you've solved the other modules",
		"Did you even read the manual?",
		"The module's broken? No I'm not! What's 9+10? 18.9992",
		"When I shred, I shred using the entire bomb. But since you SUCK, you will only need this module, and zie key button",
		"ALT+F4",
		"Did you seriously mistake me for Forget Everything?",
		"I was kidding when I told you to Forget The Colors, I guess sarcasm didn't come through that time...",
		"The Defuser expired",
		"The Expert expired",
		"You just got bamboozl- ah, wrong module",
		"Module rain. Some stay solved and others feel the pain. Module rain. 3-digit displays will die before the sin()",
		"DEpvQ0klM93dC8GMWAo5TaYGeWCZfT8Vq1qNY6o     + // /",
		"mood",
		"This message should not appear. I'll be disappointed at the Defuser if it does",
		"Did you forget about the 0 solvable module unicorn?"
    };

    public static readonly string[] WinPhrases = 
    {
        "Hey, that's pretty good",
		"*intense cat noises*",
		"While you're at it, be sure to like, comment, favorite and subscribe",
		"This is oddly calming",
		"GG m8",
		"I just came back, where we again? Oh yeah",
		"Suprisingly, your 1/91 chance went successful",
		"Did Emik fix the module or are you just that good?",
		"Did Cooldoom fix the module or are you just that good?",
		"This looks like a NUT BUTTON to me",
		"Opposite of incorrect",
		"Damn, I should ban you from solving me",
		"You haven't forgotten the colors?",
		"Do you still think it's Very Hard?",
		"I think I'm supposed to Module.HandlePass()",
		"I really hope you didn't look at the logs",
		"I really hope you didn't use an auto-solver",
		"I should have just used Azure instead of White",
		"How many shrimps do I have to eat, before it makes my gears turn pink",
		"The key spun forwards",
		"THE ANSWER IS IN THE RIGHT POSITION",
		"keyCorrect.wav",
		"Module.HandlePass()",
		"Did you use a calculator?",
		"Did you enjoy it?",
		"Please rate us 5 stars in the ModuleStore at your KTaNEPhone",
		"Alexa, play the victory tune",
		"VICTORY",
		"Maybe I should've called myself \"Write Down Colors\"",
		"E",
		"bruh moment",
		"*happy music*",
		":) good",
		"You passed the vibe check",
		"Looks like you passed your exam",
        "Successfully found USER-ANSWER in ACTUAL-ANSWER",
		"yes",
		"yesper",
		"non't",
		"This is the moment where you say \"LET'S GO!!\"",
		"You just won the game",
		"*key turned*",
		"opposite of bruh moment",
		"I was thinking about being generous this round, but you were correct anyway",
		":joy: 99% IMPOSSIBLE :joy:",
		"Forget The Colors, is this where you want to be, I just don't get it, why do you want to stay?",
		"Mood",
		"!!",
		"Now go brag to your friends",
		"PogChamp",
		"Poggers",
		"You passed with flying colors",
		"Oh, you got it right? Report it as a bug because I'm too easy, y'know?",
		"I agree, I'm just as easy as The Simpleton right beside me!",
		"Forget The Colors says: uncringe",
		"That seemed to easy for you, was colorblind enabled?",
		"And now, Souvenir",
		"I hope you wrote down the edgework-based rule for the first stage",
		"Emmm, ik that's correct",
		"Can you really say you've disabled Colorblind mode when you have a transparent bomb casing?",
		"Looks like the expert chose ally",
		"At least it's solved",
		"Clip it! Somebody highlight that or somebody clip that!",
		"Was I designed to be solved? Can't remember",
		"SPIINNN",
		"*roll credits*",
		"Hey! Your buttons are sorted- wait wrong module",
		"Do you have 200IQ or something?",
		"How would you have felt if I decided to strike?",
		"The module expired",
		"Forget The Colors expired",
		"The bomb expired",
		"A winner is you!",
		"All your module are belong to us",
		"BOB STOLE MY KEY",
		"Defuser achieved rank #1 on Being Cool:tm:"
    };
}