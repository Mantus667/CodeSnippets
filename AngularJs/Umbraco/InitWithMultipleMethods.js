//Init call which loads an entity with some dependencies.
//Only if all is loaded the screen should be shown
function init() {
	$q.all([loadTranslations(), loadDependecy1(), loadDependecy3(), loadEntity()])
    	.then(function() {
			syncTree(editCtrl.tree, "-1");
			editorState.set(editCtrl.content);
			editCtrl.loading = false;
		}, function(error) {
			notificationsService.error(loadFailedMessage, error);
		});
}
