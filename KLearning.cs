using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


public class KLerning
{
    private int states;
    private int actions;
    private double learningRate;
    private double discountFactor;
    private double explorationRate;
    private double minExplorationRate;
    private double explorationDecayRate;
    private int maxEpisodes;
    private int maxStepsPerEpisode;
    private double[,] qTable;
    private Random random;

    // Eine neue Liste zum Speichern der Belohnungen pro Episode
    public List<double> RewardsPerEpisode { get; private set; }

    public KLerning(int states, int actions, double learningRate = 0.1, double discountFactor = 0.95, double explorationRate = 1.0, double minExplorationRate = 0.01, double explorationDecayRate = 0.995, int maxEpisodes = 10000, int maxStepsPerEpisode = 200)
    {
        this.states = states;
        this.actions = actions;
        this.learningRate = learningRate;
        this.discountFactor = discountFactor;
        this.explorationRate = explorationRate;
        this.minExplorationRate = minExplorationRate;
        this.explorationDecayRate = explorationDecayRate;
        this.maxEpisodes = maxEpisodes;
        this.maxStepsPerEpisode = maxStepsPerEpisode;
        this.qTable = new double[states, actions];
        this.random = new Random();

        RewardsPerEpisode = new List<double>();
    }

    public (bool, int) Train()
    {
        for (int episode = 0; episode < maxEpisodes; episode++)
        {
            double totalReward = 0; // Gesamtbelohnung für diese Episode
            int state = random.Next(states);
            for (int step = 0; step < maxStepsPerEpisode; step++)
            {
                int action = ChooseAction(state);
                int nextState = random.Next(states);
                double reward = nextState == states - 1 ? 1.0 : 0.0;
                totalReward += reward; // Belohnung akkumulieren
                Learn(state, action, reward, nextState);
                state = nextState;
            }
            RewardsPerEpisode.Add(totalReward); // Belohnung für die Episode speichern
            UpdateExplorationRate();
            if (HasConverged())
            {
                return (true, episode);
            }
        }
        return (false, maxEpisodes);
    }

    public int ChooseAction(int state)
    {
        if (random.NextDouble() < explorationRate)
        {
            return random.Next(actions);
        }
        else
        {
            int bestAction = 0;
            double maxQValue = double.MinValue;
            for (int action = 0; action < actions; action++)
            {
                if (qTable[state, action] > maxQValue)
                {
                    maxQValue = qTable[state, action];
                    bestAction = action;
                }
            }
            return bestAction;
        }
    }

    public void Learn(int state, int action, double reward, int nextState)
    {
        double predict = qTable[state, action];
        double target = reward + discountFactor * MaxQValue(nextState);
        qTable[state, action] += learningRate * (target - predict);
    }

    private double MaxQValue(int state)
    {
        double maxQValue = double.MinValue;
        for (int action = 0; action < actions; action++)
        {
            if (qTable[state, action] > maxQValue)
            {
                maxQValue = qTable[state, action];
            }
        }
        return maxQValue;
    }

    public void UpdateExplorationRate()
    {
        explorationRate = Math.Max(minExplorationRate, explorationRate * explorationDecayRate);
    }

    public bool HasConverged(double threshold = 0.005)
    {
        for (int state = 0; state < states; state++)
        {
            double maxQValue = MaxQValue(state);
            for (int action = 0; action < actions; action++)
            {
                if (Math.Abs(qTable[state, action] - maxQValue) >= threshold)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ResizeQTable(int newStates, int newActions)
    {
        if (newStates > states || newActions > actions)
        {
            double[,] newQTable = new double[newStates, newActions];
            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < actions; j++)
                {
                    newQTable[i, j] = qTable[i, j];
                }
            }
            qTable = newQTable;
            states = newStates;
            actions = newActions;
        }
    }

   // Methode zum Speichern des Q-Tables in eine Datei
    public void SaveQTable(string filePath)
    {
        string jsonString = JsonSerializer.Serialize(qTable);
        File.WriteAllText(filePath, jsonString);
    }

    // Methode zum Laden des Q-Tables aus einer Datei
    public void LoadQTable(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);
        var deserializedData = JsonSerializer.Deserialize<double[,]>(jsonString);

        if (deserializedData != null)
        {
            qTable = deserializedData;
        }
        else
        {
            // Handle the case where the deserialization fails
            Console.WriteLine("Fehler beim Laden des Q-Tables aus der Datei.");
        }
    }

}