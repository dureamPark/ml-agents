// Trainer for C# training. One trainer per behavior.

using System;
using Unity.MLAgents.Actuators;
using Unity.Barracuda;
using UnityEngine;

namespace Unity.MLAgents
{
    internal class TrainerConfig
    {
        public int bufferSize = 1024;
        public int batchSize = 64;
        public float gamma = 0.99f;
        public int updateTargetFreq = 200;
    }

    internal class Trainer: IDisposable
    {
        ReplayBuffer m_Buffer;
        TrainingModelRunner m_ModelRunner;
        TrainingModelRunner m_TargetModelRunner;
        string m_behaviorName;
        TrainerConfig m_Config;
        int m_TrainingStep;

        public Trainer(string behaviorName, ActionSpec actionSpec, NNModel model, int seed=0, TrainerConfig config=null)
        {
            m_Config = config ?? new TrainerConfig();
            m_behaviorName = behaviorName;
            m_Buffer = new ReplayBuffer(m_Config.bufferSize);
            m_ModelRunner = new TrainingModelRunner(actionSpec, model, seed);
            m_TargetModelRunner = new TrainingModelRunner(actionSpec, model, seed);
            // copy weights from model to target model
            // m_TargetModelRunner.model.weights = m_ModelRunner.model.weights
            Academy.Instance.TrainerUpdate += Update;
        }

        public string BehaviorName
        {
            get => m_behaviorName;
        }

        public ReplayBuffer Buffer
        {
            get => m_Buffer;
        }

        public TrainingModelRunner TrainerModelRunner
        {
            get => m_ModelRunner;
        }

        public void Dispose()
        {
            Academy.Instance.TrainerUpdate -= Update;
        }

        public void Update()
        {
            if (m_Buffer.Count < m_Config.batchSize * 2)
            {
                return;
            }

            var samples = m_Buffer.SampleBatch(m_Config.batchSize);
            // states = [s.state for s in samples]
            // actions = [s.action for s in samples]
            // q_values = policy_net(states).gather(1, actions)

            // next_states = [s.next_state for s in samples]
            // rewards = [s.reward for s in samples]
            // next_state_values = target_net(non_final_next_states).max(1)[0]
            // expected_q_values = (next_state_values * GAMMA) + rewards

            // loss = MSE(q_values, expected_q_values);
            // m_ModelRunner.model = Barracuda.ModelUpdate(m_ModelRunner.model, loss);


            // Update target network
            if (m_TrainingStep % m_Config.updateTargetFreq == 0)
            {
                // copy weights
            }

            m_TrainingStep += 1;
        }
    }
}
