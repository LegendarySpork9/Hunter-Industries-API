$(function () {
    var groupHeaders = [
        {
            title: 'API Management',
            controllers: ['Audit', 'Configuration', 'Error', 'Statistic', 'Token', 'User', 'UserSettings']
        },
        {
            title: 'Assistant API',
            controllers: ['Config', 'Deletion', 'Location', 'Version']
        },
        {
            title: 'Server Status API',
            controllers: ['ServerAlert', 'ServerEvent', 'ServerInformation']
        }
    ];

    var interval = setInterval(function () {
        var resourceList = $('ul#resources');
        var resources = resourceList.children('li.resource');

        if (resources.length > 0) {
            clearInterval(interval);

            for (var i = 0; i < groupHeaders.length; i++) {
                var group = groupHeaders[i];
                var hasControllers = false;

                for (var j = 0; j < group.controllers.length; j++) {
                    if (resources.filter('[id="resource_' + group.controllers[j] + '"]').length > 0) {
                        hasControllers = true;
                        break;
                    }
                }

                if (!hasControllers) {
                    continue;
                }

                var header = $('<li class="group-header"><h2>' + group.title + '</h2></li>');
                resourceList.append(header);

                for (var j = 0; j < group.controllers.length; j++) {
                    var resource = resources.filter('[id="resource_' + group.controllers[j] + '"]');

                    if (resource.length > 0) {
                        resourceList.append(resource);
                    }
                }
            }

            resourceList.addClass('grouped');
        }
    }, 100);
});
