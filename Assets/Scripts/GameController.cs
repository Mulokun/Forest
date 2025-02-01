using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Forest
{
    public class GameController : MonoBehaviour
    {
        private GameContext game = null;

        [SerializeField] private GameData data;
        [SerializeField] private GameVisualizer visualizer;
        [SerializeField] private DrawTrees drawTree;
        [SerializeField] private TimeManagement timeManagement;

        public ModifierData D1;
        public ModifierData D2;
        public ModifierData D3;

        private Modifier gainTreeAction;
        private Modifier removeSeedAction;

        private Modifier gainSeedTick;
        private Modifier gainTreeTick;

        [Header("Debugging")]
        [SerializeField] private DebugInformationPanel debugPanel;

        protected void Awake()
        {
            game = new(data);

            timeManagement.Initialize();
            timeManagement.Resume();

            InitializeModifiers();

            visualizer.Initialize(game);
            debugPanel.Initialize(game);
        }

        private void InitializeModifiers()
        {
            /// Tick

            gainSeedTick = new(GameVariables.Seeds, Operations.Add, game[GameVariables.SeedsGainPerTick].ModifiedValue);
            game[GameVariables.SeedsGainPerTick].OnUpdate += (current, _) => gainSeedTick.Value = current;
            game.AddTick(gainSeedTick);

            gainTreeTick = new(GameVariables.Trees, Operations.Add, game[GameVariables.TreesGrownPerTick].ModifiedValue);
            game[GameVariables.TreesGrownPerTick].OnUpdate += (current, _) => gainTreeTick.Value = current;
            game.AddTick(gainTreeTick);

            /// Action

            removeSeedAction = new(GameVariables.Seeds, Operations.Add, -1);
            game.AddAction(removeSeedAction);

            gainTreeAction = new(GameVariables.Trees, Operations.Add, 1);
            game.AddAction(gainTreeAction);

            /// ---

            Tick t = new();
            t.DurationTick = game[GameVariables.TickDuration];
            t.OnTickTriggered += game.Tick;
            timeManagement.Register(t);
        }

        protected void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Action();
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                Debug.Log("Q");
                TimedModifier t1 = new(game, D1);
                timeManagement.Register(t1);
            }
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Debug.Log("W");
                TimedModifier t2 = new(game, D2);
                timeManagement.Register(t2);
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("E");
                TimedModifier t3 = new(game, D3);
                timeManagement.Register(t3);
            }
            debugPanel.UpdateValues();
        }

        public void Action()
        {
            Vector2 p = Mouse.current.position.value;
            PointerEventData eventData = new(EventSystem.current)
            {
                position = p
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count <= 0 && game[GameVariables.Seeds].ModifiedValue > 0)
            {
                UpdateActionData();
                game.Action();

                for (int i = 0; i < gainTreeAction.Value; i++)
                {
                    drawTree.DrawScreenPosition(p);
                    p = Mouse.current.position.value + (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * ((i + 1) * 10f));
                }
            }
        }

        private void UpdateActionData()
        {
            removeSeedAction.Value = -Math.Min(game[GameVariables.SeedsUsedPerAction].ModifiedValue, game[GameVariables.Seeds].ModifiedValue);
            gainTreeAction.Value = Math.Abs(removeSeedAction.Value) * game[GameVariables.TreesGrownPerSeed].ModifiedValue;
        }
    }
}
