var app = new Vue({
    data:function(){
        return{
            data:[],
            show: true,
            totalRows: 1,
            currentPage: 1,
            perPage: 5,
            pageOptions: [5, 10, 15],
            sortBy: null,
            sortDesc: false,
            sortDirection: 'asc',
            fields: [
                { key: 'LogLevel', label: 'Lvl', sortable: true},
                { key: 'LogTyp', label: 'Typ', sortable: true },
                { key: 'Time', label: 'Time',sortable:true },
                { key: 'actions', label: 'Actions' }
            ],
            filter: null,
        }
    },
    methods: {
        getData: function () {
            axios.get('/api/Logging/GetLogBySource/RRS/0-100')
                .then(x => {
                    this.data = x.data;
                }).catch(x => {
                if(x.response){
                    this.errors = x.response.data;
                }else{
                    console.log(x);
                    alert("Ein Fehler ist aufgetreten");
                }
            });
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
        // Set the initial number of items
        this.getData();
        this.totalRows = this.data.length
    },
    filters: {
        date: function (value) {
            if (!value) return '';
            var t = new Date(value);
            var dd = t.getDate();
            var mm = t.getMonth()+1; //January is 0!
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
        }
    },
    el:"#tableAnalyze"
});