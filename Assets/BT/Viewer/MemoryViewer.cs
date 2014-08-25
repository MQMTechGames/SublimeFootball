using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace MQMTech.AI.BT
{
    public class MemoryViewer
    {
        BehaviorTree _bt;

        int _xPosition;
        public int XPosition{ get { return _xPosition; } }

        int _yPosition;
        public int YPosition{ get { return _yPosition; } }

        const int kButtonWidth = 250;
        const int kButtonHeight = 20;
        
        const int kLabelWidth = 250;
        const int kLabelHeight = 20;

        public void Render(BehaviorTree bt, int xPosition, int yPosition)
        {
            _bt = bt;

            _xPosition = xPosition;
            _yPosition = yPosition;

            if(_bt.MemoryManager.GlobalMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)_xPosition, (float)_yPosition, kLabelWidth, kButtonHeight), "Global Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(_bt.MemoryManager.GlobalMemory.Map);
                
                _yPosition += 20;
            }
            
            if(_bt.MemoryManager.SharedMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)_xPosition, (float)_yPosition, kLabelWidth, kButtonHeight), "Shared Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(_bt.MemoryManager.SharedMemory.Map);
                
                _yPosition += 20;
            }
            
            if(_bt.MemoryManager.AgentMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)_xPosition, (float)_yPosition, kLabelWidth, kButtonHeight), "Agent Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(_bt.MemoryManager.AgentMemory.Map);
                
                _yPosition += 20;
            }
            
            if(_bt.MemoryManager.LocalMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)_xPosition, (float)_yPosition, kLabelWidth, kButtonHeight), "Local Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(_bt.MemoryManager.LocalMemory.Map);
                
                _yPosition += 20;
            }
        }

        void RenderMemoryVariables(Dictionary<int, object> map)
        {
            _yPosition += 20;
            
            foreach (KeyValuePair<int, System.Object> keyPair in map) 
            {
                int key = keyPair.Key;
                string keyName = MemoryKeysHashCodeManager.GetMemoryNameByHashCode(key);
                GUI.Label (new Rect ((float)_xPosition, (float)_yPosition, kLabelWidth, kButtonHeight), keyName);
                
                object value = keyPair.Value;
                string valueString = value == null ? "null" : value.ToString();
                GUI.Label (new Rect ((float)_xPosition + kLabelWidth, (float)_yPosition, kLabelWidth, kButtonHeight), valueString);
                
                _yPosition += 20;
            }
        }
    }
}
