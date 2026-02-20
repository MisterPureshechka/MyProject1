# Решение проблемы "Go to Definition" (Ctrl+Click) в Unity + VSCode

## Проблема
Не работает переход к определению классов при Ctrl+Click в VSCode для Unity проекта.

## Решение

### Шаг 1: Установите необходимые расширения VSCode

Откройте VSCode и установите следующие расширения (через Extensions: Ctrl+Shift+X):

1. **C# Dev Kit** (ID: `ms-dotnettools.csdevkit`)
   - Издатель: Microsoft
   - Это основное расширение для работы с C#

2. **C#** (ID: `ms-dotnettools.csharp`)
   - Издатель: Microsoft
   - Обычно устанавливается автоматически с C# Dev Kit

3. **Unity** (ID: `visualstudiotoolsforunity.vstuc`)
   - Издатель: Microsoft
   - Для интеграции с Unity

### Шаг 2: Настройте Unity Editor

1. Откройте Unity Editor
2. Перейдите в **Edit → Preferences** (на Mac: **Unity → Settings**)
3. Выберите **External Tools**
4. В поле **External Script Editor** выберите **Visual Studio Code**
5. Убедитесь, что включены все опции в **Generate .csproj files for:**
   - ☑ Embedded packages
   - ☑ Local packages
   - ☑ Registry packages
   - ☑ Git packages
   - ☑ Built-in packages
6. Нажмите кнопку **Regenerate project files**

### Шаг 3: Перезапустите VSCode

1. Полностью закройте VSCode
2. Откройте проект заново через Unity (Assets → Open C# Project)
   - Или откройте папку проекта напрямую в VSCode

### Шаг 4: Дождитесь загрузки OmniSharp

После открытия проекта:
1. Посмотрите в нижнюю панель VSCode
2. Должна появиться иконка пламени 🔥 или текст "OmniSharp"
3. Дождитесь сообщения "OmniSharp server is running"
4. Это может занять 1-2 минуты при первом запуске

### Шаг 5: Проверьте настройки VSCode

Убедитесь, что в `.vscode/settings.json` есть:

```json
{
    "dotnet.defaultSolution": "Pendent.sln"
}
```

(У вас эта настройка уже есть ✓)

## Дополнительные проверки

### Если проблема сохраняется:

1. **Проверьте, что .NET SDK установлен:**
   ```bash
   dotnet --version
   ```
   Должна быть версия 6.0 или выше

2. **Очистите кэш OmniSharp:**
   - Нажмите `Cmd+Shift+P` (или `Ctrl+Shift+P`)
   - Введите: `OmniSharp: Restart OmniSharp`
   - Выберите эту команду

3. **Проверьте Output панель:**
   - Откройте: View → Output
   - Выберите в выпадающем списке: "C# Dev Kit" или "OmniSharp Log"
   - Посмотрите на ошибки

4. **Пересоздайте файлы проекта в Unity:**
   - Edit → Preferences → External Tools
   - Нажмите **Regenerate project files**

5. **Перезапустите Unity и VSCode:**
   - Иногда помогает полный перезапуск обоих приложений

## Горячие клавиши для навигации

После настройки будут работать:
- **Cmd+Click** (Mac) или **Ctrl+Click** (Windows/Linux) - Go to Definition
- **F12** - Go to Definition
- **Cmd+F12** - Go to Implementation
- **Shift+F12** - Find All References

## Проверка работоспособности

Попробуйте:
1. Откройте любой .cs файл
2. Наведите курсор на класс Unity (например, `MonoBehaviour`)
3. Зажмите Ctrl (или Cmd) и кликните
4. Должен открыться декомпилированный код Unity класса

Если это работает - всё настроено правильно! ✓