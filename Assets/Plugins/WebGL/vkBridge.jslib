mergeInto(LibraryManager.library, {
  JoinGroupAndSendMessage: function () {
    if (typeof vkBridge === 'undefined') {
      console.warn("vkBridge is not loaded");
      return;
    }

    const GROUP_ID = 123456789; // ← замени на ID своей группы

    vkBridge.send('VKWebAppInit')
      .then(() => {
        return vkBridge.send('VKWebAppJoinGroup', { group_id: GROUP_ID });
      })
      .then((res) => {
        if (res.result) {
          // Предполагаем, что у тебя настроен бот, который реагирует на payload
          return vkBridge.send("VKWebAppSendPayload", {
            user_id: null, // null = текущий пользователь
            payload: { type: "joined_group", message: "Спасибо за вступление!" }
          });
        } else {
          alert("Не удалось вступить в группу");
        }
      })
      .then(() => {
        alert("Вы успешно вступили в группу. Проверьте сообщения!");
      })
      .catch((err) => {
        console.error(err);
        alert("Ошибка VK Bridge");
      });
  }
});
