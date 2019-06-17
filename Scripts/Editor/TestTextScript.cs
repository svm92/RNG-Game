using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;

public class TestTextScript {

	[Test]
	public void TestAllSentencesExistForAllLanguages() {
		foreach (TextScript.Language language in Enum.GetValues(typeof(TextScript.Language)))
        {
            TextScript.language = language; // For each language that exists
            foreach (TextScript.Sentence sentence in Enum.GetValues(typeof(TextScript.Sentence)))
            {
                TextScript.get(sentence); // Try to fetch every possible line of text
            }
        }
        // Return language to default (english) so that it doesn't conflict with other tests
        TextScript.language = TextScript.Language.ENGLISH;
	}
}
