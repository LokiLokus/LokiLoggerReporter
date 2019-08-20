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
            selData:{},
            saving:false,
            sourceStr:''
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
            this.sourceStr = location.protocol + "//" + window.location.hostname +':' + window.location.port + '/api/Logging/Log/' + row.SourceId;
            this.selData = row;
        },
        newData: function () {
            this.selData = {Version:''}
        },
        saveData: function () {
            this.saving = true;
            if(this.selData.SourceId){
                axios.put('/api/Source/Update/' + this.selData.SourceId,this.selData)
                    .then(x => {
                        this.getData();
                        this.saving = false;
                    }).catch(x => {
                    if (x.response) {
                        this.errors = x.response.data;
                    } else {
                        console.log(x)
                        alert("Ein Fehler ist aufgetreten");
                    }
                    this.saving = false;
                })
            }else{
                    axios.post('/api/Source/New',this.selData)
                        .then(x => {
                            this.selData = x.data;
                            this.getData();
                            this.saving = false;
                        }).catch(x => {
                    if (x.response) {
                        this.errors = x.response.data;
                    } else {
                        console.log(x)
                        alert("Ein Fehler ist aufgetreten");
                    }
                    this.saving = false;
                })
            }
        }
    },
    mounted: function () {
        this.getData()
    },
    computed: {
        isEmpty: function () {
            return  Object.keys(this.selData).length !== 0;
        },
        
    },
    filters: {
    },
    el:"#sourcevue"
});