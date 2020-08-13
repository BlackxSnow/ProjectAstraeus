using UI.Control;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Control
{
    public class ConfirmButton : Selectable, IPointerClickHandler
    {
        private const float RESET_TIME = 3;
#pragma warning disable 0649
        [SerializeField]
        private Sprite ConfirmSprite;
        [SerializeField]
        private ColorBlock ConfirmColourBlock;
        [SerializeField]
        private SpriteState ConfirmSpriteState;
#pragma warning restore 0649
        private Utility.Async.Timer ConfirmReset;

        [SerializeField]
        public UnityEvent OnClick = new UnityEvent();

        public enum ConfirmState
        {
            Default,
            Confirming
        }

        private ConfirmState CurrentConfirmState;

        protected override void OnEnable()
        {
            base.OnEnable();
            ConfirmReset = new Utility.Async.Timer(RESET_TIME, delegate { ResetState(); }, false);
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (CurrentConfirmState == ConfirmState.Default)
            {
                CurrentConfirmState = ConfirmState.Confirming;
                DoStateTransition(currentSelectionState, true);
                ConfirmReset.Start();
            }
            else
            {
                OnClick?.Invoke();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ConfirmReset.Stop();
            ConfirmReset = null;
        }

        private void ResetState()
        {
            CurrentConfirmState = ConfirmState.Default;
            OnDeselect(new BaseEventData(EventSystem.current));
            DoStateTransition(currentSelectionState, true);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {

            base.OnPointerDown(eventData);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            Debug.Log(currentSelectionState);
            Color color;
            Sprite newSprite;

            switch (state)
            {
                case SelectionState.Normal:
                    color = CurrentConfirmState == ConfirmState.Default ? colors.normalColor : ConfirmColourBlock.normalColor;
                    newSprite = CurrentConfirmState == ConfirmState.Default ? null : ConfirmSprite;
                    break;
                case SelectionState.Highlighted:
                    color = CurrentConfirmState == ConfirmState.Default ? colors.highlightedColor : ConfirmColourBlock.highlightedColor;
                    newSprite = CurrentConfirmState == ConfirmState.Default ? spriteState.highlightedSprite : ConfirmSpriteState.highlightedSprite;
                    break;
                case SelectionState.Selected:
                    color = CurrentConfirmState == ConfirmState.Default ? colors.selectedColor : ConfirmColourBlock.selectedColor;
                    newSprite = CurrentConfirmState == ConfirmState.Default ? spriteState.selectedSprite : ConfirmSpriteState.highlightedSprite;
                    break;
                case SelectionState.Pressed:
                    color = CurrentConfirmState == ConfirmState.Default ? colors.pressedColor : ConfirmColourBlock.pressedColor;
                    newSprite = CurrentConfirmState == ConfirmState.Default ? spriteState.pressedSprite : ConfirmSpriteState.pressedSprite;
                    break;
                case SelectionState.Disabled:
                    color = CurrentConfirmState == ConfirmState.Default ? colors.disabledColor : ConfirmColourBlock.disabledColor;
                    newSprite = CurrentConfirmState == ConfirmState.Default ? spriteState.disabledSprite : ConfirmSpriteState.disabledSprite;
                    break;
                default:
                    Debug.LogWarning("Confirm Button flowing through to default");
                    color = Color.black;
                    newSprite = null;
                    break;
            }



            if (!gameObject.activeInHierarchy)
                return;
            switch (transition)
            {
                case Transition.ColorTint:
                    StartColorTween(color * colors.colorMultiplier, instant);
                    break;
                case Transition.SpriteSwap:
                    DoSpriteSwap(newSprite);
                    break;
                case Transition.Animation:
                    break;
            }


        }
        private void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;
            targetGraphic.CrossFadeColor(targetColor, !instant ? colors.fadeDuration : 0.0f, true, true);
        }

        private void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;
            image.overrideSprite = newSprite;
        }
    } 
}



[CustomEditor(typeof(ConfirmButton))]
public class ConfirmButtonEditor : Editor
{
    SerializedProperty OnClick;

    SerializedProperty Transition;

    SerializedProperty ImageInitial;
    SerializedProperty ImageConfirm;

    SerializedProperty ColorsInitial;
    SerializedProperty ColorsConfirm;

    SerializedProperty SpritesInitial;
    SerializedProperty SpritesConfirm;

    public void OnEnable()
    {
        OnClick = serializedObject.FindProperty("OnClick");
        Transition = serializedObject.FindProperty("m_Transition");
        ImageInitial = serializedObject.FindProperty("m_TargetGraphic");
        ImageConfirm = serializedObject.FindProperty("ConfirmSprite");
        ColorsInitial = serializedObject.FindProperty("m_Colors");
        ColorsConfirm = serializedObject.FindProperty("ConfirmColourBlock");
        SpritesInitial = serializedObject.FindProperty("m_SpriteState");
        SpritesConfirm = serializedObject.FindProperty("ConfirmSpriteState");
    }

    float indentSize = 10;
    public override void OnInspectorGUI()
    {
        var script = target as ConfirmButton;

        EditorGUILayout.PropertyField(ImageInitial, new GUIContent("Initial Image"));
        EditorGUILayout.PropertyField(ImageConfirm, new GUIContent("Confirm Image"));

        EditorGUILayout.PropertyField(Transition, new GUIContent("Transition"));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(indentSize, false);
        EditorGUILayout.BeginVertical();
        switch (script.transition)
        {
            case Selectable.Transition.None:
                break;
            case Selectable.Transition.ColorTint:
                EditorGUILayout.LabelField("Default", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ColorsInitial, new GUIContent("Initial"), true);
                EditorGUILayout.LabelField("Confirm", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(ColorsConfirm, new GUIContent("Confirm"), true);
                break;
            case Selectable.Transition.SpriteSwap:
                EditorGUILayout.LabelField("Default", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(SpritesInitial, new GUIContent("Initial"), true);
                EditorGUILayout.LabelField("Confirm", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(SpritesConfirm, new GUIContent("Confirm"), true);
                break;
            case Selectable.Transition.Animation:
                break;
            default:
                break;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(OnClick, new GUIContent("OnClick"));

        serializedObject.ApplyModifiedProperties();
    }
}