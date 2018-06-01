using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject[] notes;
    private float[] _timing;
    private int[] _lineNum;

    public string filePass;
    private int _notesCount = 0;

    private AudioSource _audioSource;
    private float _startTime = 0;

    public float timeOffset = -1;

    private bool _isPlaying = false;
    public GameObject startButton;

    public Text scoreText;
    private int _score = 0;


	// Use this for initialization
	void Start ()
    {
        _audioSource = GameObject.Find("GameMusic").GetComponent<AudioSource>();
        _timing = new float[1024];
        _lineNum = new int[1024];
        LoadCSV();
        //StartGame(); //あとでボタンでできるようにする。

    }

    void Update()
    {
        if(_isPlaying)
        {
            CheckNextNotes();
            scoreText.text = _score.ToString();
        }
    }

    public void StartGame()
    {
        startButton.SetActive(false);
        _startTime = Time.time;
        _audioSource.Play();
        _isPlaying = true;
    }

    void CheckNextNotes()
    {
        while(_timing[_notesCount] + timeOffset < GetMusicTime() && _timing[_notesCount] != 0)
        {
            SpawnNotes(_lineNum[_notesCount]);
            _notesCount++;
        }
    }

    void SpawnNotes(int num) //ノーツを発生させる場所を決める.
    {
        Instantiate(notes[num], new Vector3(-0.5f * (2.0f - num), 1.5f, 10), Quaternion.identity);
    }

    void LoadCSV()
    {
        int i = 0, j;
        TextAsset csv = Resources.Load(filePass) as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while(reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(',');

            _timing[i] = float.Parse(values[0]);
            _lineNum[i] = int.Parse(values[1]);

            i++;
        }
    }

    float GetMusicTime()
    {
        return Time.time - _startTime;
    }
	
    public void GoodTimingFunc(int num)
    {
        Debug.Log("Line:" + num + " good!");
        Debug.Log(GetMusicTime());
        _score++;
    }

}
