//
//  BlankPage.swift
//  Pantry
//
//  Created by Matthias Röhricht on 03.01.25.
//

import SwiftUI
import SwiftData

struct BlankPage: View {
    @Environment(\.modelContext) private var modelContext
    @Query private var items: [Item]
    
    var body: some View {
        Text("Blank Page")
    }
}

#Preview {
    BlankPage()
        .modelContainer(for: Item.self, inMemory: true)
}
