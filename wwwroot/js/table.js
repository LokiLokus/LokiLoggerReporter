var app = new Vue({
    data:function(){
        return{
            data:[],
            show: true,
            totalRows: 1,
            currentPage: 1,
            perPage: 30,
            pageOptions: [5, 10, 15],
            sortBy: null,
            sortDesc: false,
            sortDirection: 'asc',
            fields: [
                { key: 'LogLevel', label: 'Lvl', sortable: true, class: 'text-center'},
                { key: 'LogTyp', label: 'Typ', sortable: true, class: 'text-center'},
                { key: 'Time', label: 'Time',sortable:true },
                { key: 'Message', label: 'Nachricht',sortable:true }
            ],
            filter: null,
        }
    },
    methods: {
        getData: function () {
            axios.get('/api/Logging/GetLogBySource/RRS/0-100')
                .then(x => {
                    this.data = x.data;
                    this.totalRows = this.data.length
                }).catch(x => {
                if(x.response){
                    this.errors = x.response.data;
                }else{
                    console.log(x);
                    alert("Ein Fehler ist aufgetreten");
                }
            });
        },
        expandAdditionalInfo: function(row){
            console.log(row)
            row._showDetails = !row._showDetails;
        },
      onFiltered(filteredItems) {
        // Trigger pagination to update the number of buttons/pages due to filtering
        this.totalRows = filteredItems.length;
        this.currentPage = 1
      }
    },
    computed: {
        sortOptions() {
            // Create an options list from our fields
            return this.fields
                .filter(f => f.sortable)
                .map(f => {
                    return { text: f.label, value: f.key }
                })
        }
    },
    mounted() {
        this.getData();
    },
    filters: {
        date: function (value) {
            if (!value) return '';
            var t = new Date(value);
            var dd = t.getDate();
            var mm = t.getMonth()+1;
            var yyyy = t.getFullYear();
            var hh = t.getHours();
            var mMin = t.getMinutes();
            var ss = t.getSeconds();
            
            if(dd<10){
                dd='0'+dd
            }
            if(mm<10){
                mm='0'+mm
            }
            return yyyy + '.' + mm + '.' + dd + ' - ' + hh + ':' + mMin + ':' + ss ;
        },
        message:function (value) {
            if (!value || !value.Message) return '';
            var datasets = [];
            var re = / {[\s\S]*?}/;
            var message = value.Message;
            var result = "";
            
            try{
                datasets = JSON.parse(value.Data);
            }catch(err){}
            
            if(Array.isArray(datasets)){
                for (let i = 0; i < datasets.length; i++) {
                    match = re.exec(message);
                    if(match != null){
                        var tmp = message.substr(match.index + match[0].length);
                        result += message.substr(0, match.index) + ' ' + JSON.stringify(datasets[i]);
                        
                        message = tmp;
                    }
                }
                
            }
            value.Message = result;
            
            return result.substr(0,100) + "...";
        }
    },
    el:"#tableAnalyze"
});