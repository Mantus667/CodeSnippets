function loadContact() {
			return Promise.resolve()
				.then(() => {
					if (editCtrl.isCreate) {
						return $q.resolve(editCtrl.content.contact = {
							eMail: ""
						});
					} else {
						return contactsResource.getById(editCtrl.uniqueId).then(function(response) {
							editCtrl.content.contact = response.data;
							editCtrl.ancestors.push({
								id: editCtrl.content.contact.uniqueId,
								name: editCtrl.content.contact.eMail
							});
						});
					}
				});
}
