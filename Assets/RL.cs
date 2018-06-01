using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;



public struct Sample
{
    public int state;
    public int action;
    public int next_state;
    public float reward;
    public int done;
};

public class Agent {
    private float learning_rate;
    private float e_greedy;
    private float[,] Q_table;
    public Sample [] memory;
    private int mem_counter;
    private float gamma;
    public System.Random rnd;

    public Agent(int gridsize){
        learning_rate = 0.6f;
        e_greedy = 0.9f;
        rnd = new System.Random();
        Q_table = new float[gridsize*gridsize, 4];
        memory = new Sample[100];
        mem_counter = 0;

        for (int i = 0; i < gridsize*gridsize; i++){
            for (int j = 0; j < 4; j++){
                Q_table[i, j] = 0f;
            }
        }
        gamma = 0.95f;
    }

    public int choose_action(int s){
        float[] Q = new float[4];
        float max_Q = -10000;
        int max_action = 0;
        for (int i = 0; i < 4; i++){
            if (max_Q < Q_table[s, i]){
                max_Q = Q_table[s, i];
                max_action = i;
            }
        }

        //Debug.Log(max_Q);
        //Debug.Log(max_Q);
   
        //e-greedy
        if (rnd.NextDouble() > e_greedy){
            int random_action = rnd.Next(4);
            return random_action;
        }


        return max_action;
    }

    public void learn(int s, int a, int next_state, float reward, int done){
        float Q = Q_table[s, a];

        //max Q'
        float max_Q_next = -10000f;
        for (int i = 0; i < 4; i++){
            if (max_Q_next < Q_table[next_state, i]){
                max_Q_next = Q_table[next_state, i];
            }
        }

        //Q learning
        if (done == 1){
            Q += learning_rate * (reward - Q);
        }
        else {
            Q += learning_rate * (reward + gamma * max_Q_next - Q);
        }

        Q_table[s, a] = Q;
    }

    public void add_to_memory(int ms, int ma, int mns, float mr, int mdone){
        int idx = mem_counter % 100;
        Sample sample = new Sample();
        sample.state = ms;
        sample.action = ma;
        sample.next_state = mns;
        sample.reward = mr;
        sample.done = mdone;
        memory[idx] = sample;

        mem_counter += 1;
    }

    public void saveCsv(int gridsize)
    {
        string filePath = Application.dataPath + "/QTable/" + "saved_data.csv";
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine("action0,action1,action2,action3");

        for (int i = 0; i < (gridsize * gridsize); i++)
        {
            writer.WriteLine(Q_table[i, 0].ToString() + "," +
            Q_table[i, 1].ToString() + "," +
            Q_table[i, 2].ToString() + "," +
            Q_table[i, 3].ToString());

        }
        writer.Flush();
        writer.Close();
    }

    public void readCsv(int gridsize)
    {
        float action0, action1, action2, action3;
        string[] lines = File.ReadAllText(Application.dataPath + "/QTable/" + "saved_data.csv").Split("\n"[0]);
        //string[] lines = file.text.Split ("\n" [0]);
        for (int i = 0; i < (gridsize * gridsize); i++)
        {
            string[] actions = lines[i].Split(","[0]);

            float.TryParse(actions[0], out this.Q_table[i, 0]);
            float.TryParse(actions[1], out this.Q_table[i, 1]);
            float.TryParse(actions[2], out this.Q_table[i, 2]);
            float.TryParse(actions[3], out this.Q_table[i, 3]);

        }
    }

}

public class RL : MonoBehaviour {

    private Rigidbody rg;
    private Agent agent;
    private int state;
    private float reward;
    private int gridsize;
    private int done;
    private int t;
    int frames = 0;
    public int frameRate;
    int action = 0;
    public bool Import;
    public bool Export;
    private Vector3 startPos;

    public int exited = 0 ;

    // Use this for initialization
    void Start () {
        startPos = transform.position;
        gridsize = 50;
        agent = new Agent(gridsize);
        if (Import == true)
        {
            agent.readCsv(gridsize);
        }
        env_reset();
        t = 0;
	}

    int env_reset() {
        //Vector3 initial_position = new Vector3(25f, 0.5f, 25f);
        Vector3 initial_position = startPos;
        transform.position = initial_position;

        //Convert to grid
        int x = Mathf.RoundToInt(transform.position.x);
        int z = Mathf.RoundToInt(transform.position.z);
        Debug.Log(x);
        Debug.Log(z);
        int s_idx = x * gridsize + z;

        //Return index
        done = 0;
        reward = 0;
        return s_idx;
    }

    int env_step(int a, Vector3 initial_pos) {
        Vector3 diff = new Vector3(0.0f, 0.0f, 0.0f);

        Vector3 check_final = new Vector3(0.0f, 0.0f, 0.0f);

        if (a == 0){
            //BACK
            diff.x = -2f / frameRate;
            check_final.x = -2f;
        }

        else if (a == 1)
        {
            //FRONT
            diff.x = 2f / frameRate;
            check_final.x = 2f;
        }

        else if (a == 2)
        {
            //LEFT
            diff.z = -2f / frameRate;
            check_final.z = -2f;
        }

        else if (a == 3) {
            //RIGHT
            diff.z = 2f / frameRate;
            check_final.z = 2f;
        }

        Vector3 target_pos = transform.position + diff;
        Vector3 test_pos = initial_pos + check_final/2;


        //Check if target pose collides with wall
        //float radius = 1;
        //if 
        Collider[] blockTest = Physics.OverlapBox(test_pos, new Vector3(0.5f, 0.5f, 0.5f));
        if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
        {
            transform.position = target_pos;
            reward = -1;

            if (blockTest.Where(col => col.gameObject.tag == "Goal").ToArray().Length == 1)
            {
                done = 1;
                reward = 100;
                print("You are out!!!");
                exited++;
            }
            //if (blockTest.Where(col => col.gameObject.tag == "pit").ToArray().Length == 1)
            //{
            //    Done();
            //    SetReward(-1f);
            //}
        }
        else
        {
            reward = -10;
        }

        int x = Mathf.RoundToInt(transform.position.x);
        int z = Mathf.RoundToInt(transform.position.z);
        int next_state = x * gridsize + z;

        return next_state;




    }

    int time = 0;
    Vector3 init_pos;

	// Update is called once per frame
    void Update () {

        if (frames % frameRate == 0)
        {
            if (done == 1)
            {
                state = env_reset();
                if (Export == true)
                {
                    agent.saveCsv(gridsize);
                }
            }
            action = agent.choose_action(state);
            init_pos = transform.position;
        }

        int next_state = env_step(action, init_pos);
        if (frames % frameRate == 0)
        {
            agent.add_to_memory(state, action, next_state, reward, done);

            agent.learn(state, action, next_state, reward, done);

            if (time > 100)
            {
                for (int i = 0; i < 5; i++)
                {
                    int sample = agent.rnd.Next(100);
                    int sample_state = agent.memory[sample].state;
                    int sample_action = agent.memory[sample].action;
                    int sample_n_state = agent.memory[sample].next_state;
                    float sample_reward = agent.memory[sample].reward;
                    int sample_done = agent.memory[sample].done;
                    agent.learn(sample_state, sample_action, sample_n_state, sample_reward, sample_done);
                }
            }


            state = next_state;
            time += 1;
            if (time % 1000 == 0){
                done = 1;
            }
                
        }
        frames++;
	}
}
