# Quick AI 🧠✨

<div align="center">
  <img src="./Media/app_icon.ico" alt="Quick AI" width="120" height="120">
  <p>A lightweight desktop application for interacting with Azure OpenAI services</p>
</div>

## ✨ Features

Quick AI is a desktop application that provides a clean, distraction-free interface for interacting with Azure OpenAI's language models. It offers two specialized modes:

- **General AI Assistant** (F1) - Get answers to questions, generate content, brainstorm ideas
- **Grammar Correction** (F2) - Polish and improve text in any language

### Configuration

1. Click "Configurações" (Settings) in the top-right corner
2. Enter your Azure OpenAI:
   - Endpoint URL
   - API Key
   - Deployment Name
3. Save with Ctrl+S or click the save button

## 💬 How to Use

### General AI Assistant (F1)

Simply type your question or prompt and press Ctrl+Enter or click the send button. Examples:

- "What's the capital of France?"
- "Write a limerick about programming"
- "Explain quantum computing to a 10-year old"

### Grammar Correction (F2)

Paste text you want to improve and press Ctrl+Enter. The AI will:

1. Identify the language
2. Fix grammatical errors
3. Improve phrasing and clarity
4. Display a summary of all changes made

### Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| F1 | Switch to General Mode |
| F2 | Switch to Grammar Mode |
| Ctrl+Enter | Send prompt |
| Shift+Enter | Send prompt |
| Ctrl+N | Clear & start new prompt |
| Esc | Minimize window |
| Ctrl+S | Save settings (in Settings window) |

## 🛠️ Technical Details

Quick AI is built using:

- .NET 9.0 with WPF
- Azure.AI.OpenAI SDK for API communication
- Registry-based settings storage

## 📋 Requirements

- Windows 10/11
- .NET 9.0 Runtime
- Azure OpenAI API access with a deployed model
- Minimal system resources (runs well on any modern hardware)


---

<div align="center">
  <p>Made with ❤️ by <a href="https://github.com/fellipeborges">Fellipe Borges</a></p>
</div>