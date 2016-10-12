﻿using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;

class LevelManager : MonoBehaviour {

    private List<Level> levels = new List<Level>();
    private int level;

    void Awake() {
        print("LevelManager Start");
        TextAsset text = Resources.Load("GameData/Entry") as TextAsset;
        print(text.text);
        var levelList = JSON.Parse(text.text);
        int count = levelList["levelCount"].AsInt;
        for(int i = 0; i < count; ++i) {
            Level level = new Level();
            level.name = levelList["levels"][i]["name"];
            var levelConfig = JSON.Parse(
                (Resources.Load("GameData/"+ levelList["levels"][i]["fileName"])
                as TextAsset).text);
            for(int j = 0; j < levelConfig["waveCount"].AsInt; ++j) {
                var waveJSON = levelConfig["waves"][j];
                Level.Wave wave = new Level.Wave();
                wave.waveDuring = waveJSON["waveDuring"].AsInt;
                for(int k = 0; k < waveJSON["spawnPointCount"].AsInt; ++k) {
                    var spJSON = waveJSON["spawnPoints"][k];
                    Level.Wave.SpawnPoint sp = new Level.Wave.SpawnPoint();
                    sp.interval = spJSON["interval"].AsFloat;
                    print(sp.interval);
                    sp.number = spJSON["number"].AsInt;
                    sp.prefab = spJSON["prefab"];
                    sp.speed = spJSON["speed"].AsFloat;
                    sp.damage = spJSON["damage"].AsFloat;
                    sp.position = new Vector3(spJSON["position"][0].AsFloat, spJSON["position"][1].AsFloat, spJSON["position"][2].AsFloat);
                    wave.spawnPoints.Add(sp);
                }
                level.waves.Add(wave);
            }
            this.levels.Add(level);
        }
    }

    public void JumpLevel(int number) {
        this.level = number;
        print("JUMP TO " + number.ToString());
        SceneManager.LoadScene("AutoGeneratedLevel");
        return;
    }

    public void JumpToTutorial() {
        this.level = 0;
        SceneManager.LoadScene("Tutorial");
        return;
    }

    public Level GetCurrentLevel() {
        return this.levels[this.level];
    }
    public void ReloadLevel() {
        this.JumpLevel(this.level);
    }

    public bool JumpToNextLevel() {
        if (this.level >= this.levels.Count)
            return false;
        else {
            this.JumpLevel(this.level);
            return true;
        }
    }

    public List<string> GetLevelNames() {
        List<string> res = new List<string>();
        foreach(Level level in this.levels) {
            res.Add(level.GetName());
        }
        return res;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
