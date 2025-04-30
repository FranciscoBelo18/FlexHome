// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using UnityEngine.UI;

namespace Mediapipe.Unity
{
  public class Screen : MonoBehaviour
  {
    [SerializeField] private RawImage _screen;

    private ImageSource _imageSource;

    // Variáveis para monitorar mudanças na resolução da tela
    private int _lastScreenWidth;
    private int _lastScreenHeight;

    public Texture texture
    {
      get => _screen.texture;
      set => _screen.texture = value;
    }

    public UnityEngine.Rect uvRect
    {
      set => _screen.uvRect = value;
    }

    public void Initialize(ImageSource imageSource)
    {
      _imageSource = imageSource;

      Resize(_imageSource.textureWidth, _imageSource.textureHeight);
      Rotate(_imageSource.rotation.Reverse());
      ResetUvRect(RunningMode.Async);
      texture = imageSource.GetCurrentTexture();

      // Salva o estado atual da tela para comparar em Update()
      _lastScreenWidth = UnityEngine.Screen.width;
      _lastScreenHeight = UnityEngine.Screen.height;
    }

    public void Resize(int textureWidth, int textureHeight)
    {
      var canvas = _screen.canvas;

      if (canvas == null)
      {
        Debug.LogWarning("Canvas not found for RawImage. Using fixed size.");
        _screen.rectTransform.sizeDelta = new Vector2(textureWidth, textureHeight);
        return;
      }

      // Utiliza o nome completo para evitar conflito com o nome da classe (Screen)
      var screenWidth = UnityEngine.Screen.width;
      var screenHeight = UnityEngine.Screen.height;
      float screenAspect = (float)screenWidth / screenHeight;
      float textureAspect = (float)textureWidth / textureHeight;

      float width, height;

      if (textureAspect > screenAspect)
      {
        // A textura é mais larga que a tela, ajusta para preencher a largura
        width = screenWidth;
        height = screenWidth / textureAspect;
      }
      else
      {
        // A textura é mais alta que a tela, ajusta para preencher a altura
        height = screenHeight;
        width = screenHeight * textureAspect;
      }

      _screen.rectTransform.sizeDelta = new Vector2(width, height);
    }

    public void Rotate(RotationAngle rotationAngle)
    {
      _screen.rectTransform.localEulerAngles = rotationAngle.GetEulerAngles();
    }

    public void ReadSync(Experimental.TextureFrame textureFrame)
    {
      if (!(texture is Texture2D))
      {
        texture = new Texture2D(_imageSource.textureWidth, _imageSource.textureHeight, TextureFormat.RGBA32, false);
        ResetUvRect(RunningMode.Sync);
      }
      textureFrame.CopyTexture(texture);
    }

    private void ResetUvRect(RunningMode runningMode)
    {
      var rect = new UnityEngine.Rect(0, 0, 1, 1);

      if (_imageSource.isVerticallyFlipped && runningMode == RunningMode.Async)
      {
        // No modo Async, não é necessário flipar verticalmente, pois a imagem será copiada na CPU.
        rect = FlipVertically(rect);
      }

      if (_imageSource.isFrontFacing)
      {
        // Flip na imagem (não na tela) horizontalmente.
        // Leva em consideração que a imagem será rotacionada posteriormente.
        var rotation = _imageSource.rotation;

        if (rotation == RotationAngle.Rotation0 || rotation == RotationAngle.Rotation180)
        {
          rect = FlipHorizontally(rect);
        }
        else
        {
          rect = FlipVertically(rect);
        }
      }

      uvRect = rect;
    }

    private UnityEngine.Rect FlipHorizontally(UnityEngine.Rect rect)
    {
      return new UnityEngine.Rect(1 - rect.x, rect.y, -rect.width, rect.height);
    }

    private UnityEngine.Rect FlipVertically(UnityEngine.Rect rect)
    {
      return new UnityEngine.Rect(rect.x, 1 - rect.y, rect.width, -rect.height);
    }

    // Monitoramento em tempo real para detectar mudanças na resolução/rotação da tela
    private void Update()
    {
      if (UnityEngine.Screen.width != _lastScreenWidth || UnityEngine.Screen.height != _lastScreenHeight)
      {
        _lastScreenWidth = UnityEngine.Screen.width;
        _lastScreenHeight = UnityEngine.Screen.height;
        Resize(_imageSource.textureWidth, _imageSource.textureHeight);
      }
    }
  }
}
