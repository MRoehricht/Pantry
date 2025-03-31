//
//  TagFilterView.swift
//  Pantry
//
//  Created by Matthias Röhricht on 31.03.25.
//

import SwiftUI

struct TagFilterView: View {
    @Binding var selectedTags: [String]
    var allTags: [String]

    var body: some View {
        ScrollView(.horizontal, showsIndicators: false) {
            HStack {
                ForEach(allTags, id: \.self) { tag in
                    Button(action: {
                        if selectedTags.contains(tag) {
                            selectedTags.removeAll { $0 == tag }
                        } else {
                            selectedTags.append(tag)
                        }
                    }) {
                        Text(tag)
                            .padding()
                            .background(selectedTags.contains(tag) ? Color.blue : Color.gray)
                            .foregroundColor(.white)
                            .cornerRadius(8)
                    }
                }
            }
        }
        .safeAreaPadding(.leading,20)      
    }
}
