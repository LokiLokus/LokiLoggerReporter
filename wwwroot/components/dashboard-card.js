Vue.component('dashboard-card', {
    
    props:['color','count','message','link'],
    
    template: '<div class="col-xl-3 col-sm-6 mb-3">\n' +
        '        <div :class="color" class="card text-white o-hidden h-100">\n' +
        '            <div class="card-body">\n' +
        '                <div class="mr-5">{{count}} {{message}} </div>\n' +
        '            </div>\n' +
        '            <a class="card-footer text-white clearfix small z-1" :href="link">\n' +
        '                <span class="float-left">View Details</span>\n' +
        '                <span class="float-right">\n' +
        '                <i class="fas fa-angle-right"></i>\n' +
        '            </span>\n' +
        '            </a>\n' +
        '        </div>\n' +
        '    </div>'
});