var app = new Vue({
    data:function(){
        return{
            fields:[
                { key: 'Name', label: 'Name', sortable: true, sortDirection: 'desc' },
                { key: 'Version', label: 'Version', sortable: true, sortDirection: 'desc' },
                { key: 'Tag', label: 'Tag', sortable: true, sortDirection: 'desc' }
            ],
            data:[],
            loaded:false,
            selData:{}
        }
    },
    methods: {
        getData: function () {
            axios.get('/api/Source/All')
                .then(x => {
                    this.data = x.data;
                    this.loaded = true;
            }).catch(x => {
                if(x.response){
                this.errors = x.response.data;
            }else{
                console.log(x)
                alert("Ein Fehler ist aufgetreten");
            }
        });
        },
        select:function (row) {
            this.selData = row;
        }
    },
    mounted: function () {
        this.getData()
    },
    computed: {
        isEmpty: function () {
            return isEmpty(this.selData)
        },
        
    },
    filters: {
    },
    el:"#sourcevue"
});